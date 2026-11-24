#if UNITY_EDITOR || DEVELOPMENT_BUILD
using System.Collections;
using System.Linq;
using Fusion;
using UnityEngine;

namespace CoreClash.Lesson02
{
    // Opt-in integration verification; never runs during normal play.
    public class Lesson02SmokeTest : MonoBehaviour
    {
        private void Start()
        {
            if (System.Environment.GetCommandLineArgs().Contains("-lesson02Smoke")) StartCoroutine(Run());
        }

        private IEnumerator Run()
        {
            yield return new WaitForSeconds(2f);
            string[] args = System.Environment.GetCommandLineArgs();
            int index = System.Array.IndexOf(args, "-lesson02Room");
            string room = index >= 0 && index + 1 < args.Length ? args[index + 1] : "Lesson12Verification";
            Lesson02NetworkManager manager = FindFirstObjectByType<Lesson02NetworkManager>();
            manager.StartRoom(room, "Verify" + System.Diagnostics.Process.GetCurrentProcess().Id);
            float deadline = Time.realtimeSinceStartup + 60f;
            Lesson02Player local = null;
            Lesson02Player[] players = null;
            Lesson02CorePickup[] cores = null;
            while (Time.realtimeSinceStartup < deadline)
            {
                players = FindObjectsByType<Lesson02Player>(FindObjectsSortMode.None)
                    .Where(p => p.Object != null && p.Object.IsValid).OrderBy(p => p.Object.InputAuthority.RawEncoded).ToArray();
                local = players.FirstOrDefault(p => p.Object.HasInputAuthority);
                cores = FindObjectsByType<Lesson02CorePickup>(FindObjectsSortMode.None);
                if (local != null && players.Length == 2 && cores.Length == 5) break;
                yield return null;
            }
            if (local == null || players.Length != 2 || cores.Length != 5)
            { Finish(false, $"connection players={players?.Length} cores={cores?.Length}"); yield break; }

            // Let both peers finish spawning before triggering real collider callbacks.
            yield return new WaitForSeconds(3f);
            Lesson02Player attacker = players[0], target = players[1];
            Lesson02CorePickup core = cores.OrderBy(c => c.transform.position.x).First();
            int scoreBefore = attacker.Cores;
            if (local == attacker) yield return MoveTo(manager, local, core.transform.position);
            deadline = Time.realtimeSinceStartup + 8f;
            while (attacker.Cores == scoreBefore && Time.realtimeSinceStartup < deadline) yield return null;
            bool collected = attacker.Cores == scoreBefore + 1 && !core.IsAvailable;
            if (local == attacker) yield return MoveTo(manager, local, new Vector3(-8f, 1f, 0f));
            deadline = Time.realtimeSinceStartup + 8f;
            while (!core.IsAvailable && Time.realtimeSinceStartup < deadline) yield return null;
            bool respawned = core.IsAvailable && attacker.Cores == scoreBefore + 1;

            // Put both owners at known positions, then exercise TryMelee's real overlap and RPC.
            yield return MoveTo(manager, local, local == attacker ? new Vector3(0f, 1f, 0f) : new Vector3(0f, 1f, 1.5f));
            deadline = Time.realtimeSinceStartup + 20f;
            while ((Vector3.Distance(attacker.transform.position, new Vector3(0f, 1f, 0f)) > .3f ||
                    Vector3.Distance(target.transform.position, new Vector3(0f, 1f, 1.5f)) > .3f) && Time.realtimeSinceStartup < deadline) yield return null;
            yield return new WaitForSeconds(2f);
            if (local == attacker)
            {
                NetworkButtons buttons = default;
                buttons.Set((int)Lesson02Button.Attack, true);
                manager.VerificationInput = new Lesson02InputData { Move = Vector2.up * .01f, Buttons = buttons };
            }
            bool effect = false;
            deadline = Time.realtimeSinceStartup + 2f;
            while (Time.realtimeSinceStartup < deadline)
            {
                effect |= GameObject.Find("MeleeFlash") != null;
                yield return null;
            }
            manager.VerificationInput = new Lesson02InputData();
            bool melee = target.Health == 75;
            bool knockback = Vector3.Distance(target.transform.position, new Vector3(0f, 1f, 1.5f)) > .3f;
            bool passed = collected && respawned && melee && effect && knockback;
            string details = $"players={players.Length} cores={cores.Length} collected={collected} respawned={respawned} melee={melee} effect={effect} knockback={knockback} position={target.transform.position} score={attacker.Cores} hp={target.Health}";
            Debug.Log($"[Lesson02Smoke] RESULT {details}");
            yield return new WaitForSeconds(5f);
            Finish(passed, details);
        }

        private static IEnumerator MoveTo(Lesson02NetworkManager manager, Lesson02Player player, Vector3 destination)
        {
            float deadline = Time.realtimeSinceStartup + 15f;
            while (Time.realtimeSinceStartup < deadline)
            {
                Vector3 delta = destination - player.transform.position;
                Vector2 direction = new Vector2(delta.x, delta.z);
                if (direction.magnitude < .15f) break;
                manager.VerificationInput = new Lesson02InputData { Move = direction.normalized };
                yield return null;
            }
            manager.VerificationInput = new Lesson02InputData();
        }

        private static void Finish(bool pass, string details)
        {
            Debug.Log($"[Lesson02Smoke] {(pass ? "PASS" : "FAIL")} {details}");
            Application.Quit(pass ? 0 : 2);
        }
    }
}

#endif

