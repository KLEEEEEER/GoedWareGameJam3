using GoedWareGameJam3.MonoBehaviours.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GoedWareGameJam3.Core.Interfaces
{
    public interface IJumpable
    {
        bool CanBeJumped { get; }
        void Jump(PlayerJumpInteraction playerJumpInteraction);
    }
}