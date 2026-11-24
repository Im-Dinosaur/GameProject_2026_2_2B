using Fusion;
using UnityEngine;

namespace CoreClash.Lesson02
{
    [RequireComponent(typeof(NetworkObject), typeof(Collider))]
    public class Lesson02CorePickup : NetworkBehaviour
    {
        [SerializeField] private float respawnSeconds = 5f;
        // Replicated availability and respawn deadline.
        [Networked] public NetworkBool IsAvailable { get; private set; }
        [Networked] private TickTimer RespawnTimer { get; set; }

        public override void Spawned()
        {
            if (Object.HasStateAuthority) IsAvailable = true;
            ApplyVisibleState();
        }

        public override void FixedUpdateNetwork()
        {
            if (Object.HasStateAuthority && !IsAvailable && RespawnTimer.Expired(Runner)) IsAvailable = true;
        }

        public override void Render()
        {
            transform.Rotate(0f, 100f * Time.deltaTime, 0f);
            ApplyVisibleState();
        }

        private void OnTriggerEnter(Collider other)
        {
            Lesson02Player player = other.GetComponentInParent<Lesson02Player>();
            if (player != null && player.Object.HasInputAuthority) RPC_Collect(player.Object.InputAuthority);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority)]
        private void RPC_Collect(PlayerRef playerRef)
        {
            if (!IsAvailable || !Runner.TryGetPlayerObject(playerRef, out NetworkObject playerObject)) return;
            Lesson02Player player = playerObject.GetComponent<Lesson02Player>();
            if (player == null) return;
            player.RequestAddCore(1);
            IsAvailable = false;
            RespawnTimer = TickTimer.CreateFromSeconds(Runner, respawnSeconds);
        }

        private void ApplyVisibleState()
        {
            foreach (Renderer item in GetComponentsInChildren<Renderer>()) item.enabled = IsAvailable;
            GetComponent<Collider>().enabled = IsAvailable;
        }
    }
}
