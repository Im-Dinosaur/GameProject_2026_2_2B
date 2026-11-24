#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System;
using System.Collections;
using System.Linq;
using Fusion;
using UnityEngine;

namespace CoreClash.Lesson02
{
    // Opt-in network integration fixture; ordinary play never calls this code.
    internal static class Lesson13LifecycleVerification
    {
        internal static IEnumerator Run()
        {
            yield return new WaitForSeconds(2f);
            string[] args = Environment.GetCommandLineArgs();
            int index = Array.IndexOf(args, "-lesson02Room");
            string room = index >= 0 && index + 1 < args.Length ? args[index + 1] : "Lesson13Lifecycle";
            Lesson02NetworkManager manager = UnityEngine.Object.FindFirstObjectByType<Lesson02NetworkManager>();
            manager.StartRoom(room, "Lifecycle" + System.Diagnostics.Process.GetCurrentProcess().Id);
            Lesson02Player[] players = Array.Empty<Lesson02Player>();
            float deadline = Time.realtimeSinceStartup + 60f;
            while (Time.realtimeSinceStartup < deadline)
            {
                players = UnityEngine.Object.FindObjectsByType<Lesson02Player>(FindObjectsSortMode.None)
                    .Where(p => p.Object != null && p.Object.IsValid)
                    .OrderBy(p => p.Object.InputAuthority.RawEncoded).ToArray();
                if (players.Length == 2 && players.Any(p => p.Object.HasInputAuthority)) break;
                yield return null;
            }
            if (players.Length != 2) { Finish(false, "two players missing"); yield break; }
            Lesson02Player local = players.First(p => p.Object.HasInputAuthority);
            Lesson02Player attacker = players[0], target = players[1];
            bool driver = local == attacker;
            manager.VerificationInput = new Lesson02InputData();
            yield return new WaitForSeconds(3f);
            string expectedLocalName = "Lifecycle" + System.Diagnostics.Process.GetCurrentProcess().Id;
            bool namesSynced = local.PlayerDisplayName == expectedLocalName &&
                players.All(p => p.PlayerDisplayName.StartsWith("Lifecycle", StringComparison.Ordinal)) &&
                players.Select(p => p.PlayerDisplayName).Distinct().Count() == 2;
            bool initial = attacker.Ammo == 6 && target.Ammo == 6 && attacker.kills == 0 && target.Health == 100;
            if (!initial) { Finish(false, "unexpected initial replicated state"); yield break; }

            if (driver) target.RequestDamage(100, Vector3.forward, attacker.Object.InputAuthority);
            deadline = Time.realtimeSinceStartup + 5f;
            while ((!target.IsDead || attacker.kills != 1) && Time.realtimeSinceStartup < deadline) yield return null;
            bool death = target.IsDead && target.Health == 0 && attacker.kills == 1;
            if (!death) { Finish(false, "death or kill RPC missing"); yield break; }
            if (driver) target.RequestDamage(100, Vector3.forward, attacker.Object.InputAuthority);
            yield return new WaitForSeconds(.3f);
            Vector3 deadPosition = target.transform.position;
            if (local == target) manager.VerificationInput = new Lesson02InputData { Move = Vector2.right };
            yield return new WaitForSeconds(.5f);
            manager.VerificationInput = new Lesson02InputData();
            bool deadStopped = Vector3.Distance(deadPosition, target.transform.position) < .1f;
            bool singleKill = attacker.kills == 1 && target.Health == 0;

            deadline = Time.realtimeSinceStartup + 5f;
            while (target.IsDead && Time.realtimeSinceStartup < deadline) yield return null;
            float spawnX = (target.Object.InputAuthority.RawEncoded % 5 - 2) * 2f;
            // Transform interpolation may trail the status snapshot on the remote peer.
            deadline = Time.realtimeSinceStartup + 1f;
            while (Vector3.Distance(target.transform.position, new Vector3(spawnX, 1f, 0f)) >= .2f &&
                   Time.realtimeSinceStartup < deadline) yield return null;
            bool respawn = !target.IsDead && target.Health == 100 && target.IsInvincible &&
                Vector3.Distance(target.transform.position, new Vector3(spawnX, 1f, 0f)) < .2f;
            Debug.Log($"[Lesson13Lifecycle] respawn hp={target.Health} dead={target.IsDead} invincible={target.IsInvincible} position={target.transform.position} expectedX={spawnX}");
            if (driver) target.RequestDamage(20, Vector3.forward, attacker.Object.InputAuthority);
            yield return new WaitForSeconds(.4f);
            bool protectedFromDamage = target.Health == 100;
            deadline = Time.realtimeSinceStartup + 4f;
            while (target.IsInvincible && Time.realtimeSinceStartup < deadline) yield return null;
            bool protectionExpired = !target.IsInvincible;
            if (driver) target.RequestDamage(20, Vector3.forward, attacker.Object.InputAuthority);
            deadline = Time.realtimeSinceStartup + 3f;
            while (target.Health == 100 && Time.realtimeSinceStartup < deadline) yield return null;
            bool vulnerable = target.Health == 80 && attacker.kills == 1;

            // Set up an empty magazine on its owner. This is fixture state, not a firing test:
            // the lecture has not supplied the projectile prefab yet.
            if (driver) typeof(Lesson02Player).GetProperty(nameof(Lesson02Player.Ammo)).SetValue(attacker, 0);
            deadline = Time.realtimeSinceStartup + 3f;
            while (attacker.Ammo != 0 && Time.realtimeSinceStartup < deadline) yield return null;
            yield return new WaitForSeconds(.3f);
            if (driver)
            {
                NetworkButtons buttons = default;
                buttons.Set((int)Lesson02Button.Reload, true);
                manager.VerificationInput = new Lesson02InputData { Buttons = buttons };
            }
            yield return new WaitForSeconds(.2f);
            manager.VerificationInput = new Lesson02InputData();
            yield return new WaitForSeconds(.6f);
            bool reloadWaited = attacker.Ammo == 0;
            deadline = Time.realtimeSinceStartup + 4f;
            while (attacker.Ammo != 6 && Time.realtimeSinceStartup < deadline) yield return null;
            bool reloaded = attacker.Ammo == 6;
            string details = $"names={namesSynced} death={death} singleKill={singleKill} deadStopped={deadStopped} respawn={respawn} invincible={protectedFromDamage} expired={protectionExpired} vulnerable={vulnerable} reloadWaited={reloadWaited} reloaded={reloaded} kills={attacker.kills} hp={target.Health} ammo={attacker.Ammo}";
            bool pass = namesSynced && death && singleKill && deadStopped && respawn && protectedFromDamage && protectionExpired && vulnerable && reloadWaited && reloaded;
            Debug.Log("[Lesson13Lifecycle] RESULT " + details);
            yield return new WaitForSeconds(5f);
            Finish(pass, details);
        }

        private static void Finish(bool pass, string details)
        {
            Debug.Log($"[Lesson13Lifecycle] {(pass ? "PASS" : "FAIL")} {details}");
            Application.Quit(pass ? 0 : 2);
        }
    }
}
#endif
