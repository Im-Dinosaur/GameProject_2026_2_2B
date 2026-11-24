using System.Collections;
using Fusion;
using UnityEngine;

namespace CoreClash.Lesson02
{
    [RequireComponent(typeof(NetworkObject), typeof(NetworkTransform))]
    public class Lesson02Player : NetworkBehaviour
    {
        [SerializeField] private float meleeRange = 2.2f;
        [SerializeField] private int meleeDamage = 25;
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float turnSpeed = 14f;

        // State Authority owns the replicated gameplay state.
        [Networked] public int Health { get; private set; }
        [Networked] public int Cores { get; private set; }
        [Networked] private TickTimer MeleeCooldown { get; set; }
        [Networked] private NetworkButtons PreviousButtons { get; set; }

        public override void Spawned()
        {
            if (!Object.HasStateAuthority) return;
            Health = 100;
            Runner.SetPlayerObject(Object.InputAuthority, Object);
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority || !GetInput(out Lesson02InputData input)) return;
            ApplyMovement(input.Move);
            if (input.Buttons.WasPressed(PreviousButtons, (int)Lesson02Button.Attack)) TryMelee();
            PreviousButtons = input.Buttons;
        }

        private void ApplyMovement(Vector2 input)
        {
            Vector3 direction = new Vector3(input.x, 0f, input.y);
            if (direction.sqrMagnitude < .001f) return;
            direction.Normalize();
            transform.position += direction * moveSpeed * Runner.DeltaTime;
            Quaternion rotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, turnSpeed * Runner.DeltaTime);
        }

        public void RequestAddCore(int amount) => RPC_AddCore(amount);
        public void RequestDamage(int amount, Vector3 direction) => RPC_Damage(amount, direction);

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_AddCore(int amount)
        {
            Cores += Mathf.Max(0, amount);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_Damage(int amount, Vector3 direction)
        {
            Health = Mathf.Max(0, Health - amount);
            transform.position += direction.normalized * .45f;
        }

        private void TryMelee()
        {
            if (!MeleeCooldown.ExpiredOrNotRunning(Runner)) return;
            MeleeCooldown = TickTimer.CreateFromSeconds(Runner, .6f);
            RPC_PlayMeleeEffect();
            Vector3 center = transform.position + transform.forward * 1.2f;
            foreach (Collider hit in Physics.OverlapSphere(center, meleeRange))
            {
                Lesson02Player target = hit.GetComponentInParent<Lesson02Player>();
                if (target == null || target == this) continue;
                target.RequestDamage(meleeDamage, transform.forward);
                break;
            }
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_PlayMeleeEffect()
        {
            StartCoroutine(PlayMeleeEffect());
        }

        private IEnumerator PlayMeleeEffect()
        {
            GameObject flash = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            flash.name = "MeleeFlash";
            Destroy(flash.GetComponent<Collider>());

            Renderer flashRenderer = flash.GetComponent<Renderer>();
            Renderer sourceRenderer = GetComponentInChildren<Renderer>();
            Material material = sourceRenderer != null && sourceRenderer.sharedMaterial != null
                ? new Material(sourceRenderer.sharedMaterial)
                : flashRenderer.material;
            material.color = new Color(0.1f, 1f, 0.9f, 1f);
            flashRenderer.material = material;

            Vector3 start = transform.position + Vector3.up * 0.8f + transform.forward * 0.7f;
            Vector3 end = transform.position + Vector3.up * 0.8f + transform.forward * 2.0f;
            float duration = 0.3f;

            for (float time = 0f; time < duration; time += Time.deltaTime)
            {
                float t = time / duration;
                flash.transform.position = Vector3.Lerp(start, end, t);
                flash.transform.rotation = transform.rotation;
                flash.transform.localScale = Vector3.Lerp(
                    new Vector3(0.25f, 0.25f, 0.25f),
                    new Vector3(2.4f, 0.18f, 0.7f), t);
                yield return null;
            }

            Destroy(flash);
            if (material != null) Destroy(material);
        }
    }
}
