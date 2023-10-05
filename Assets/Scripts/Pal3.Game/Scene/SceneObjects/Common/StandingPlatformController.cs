﻿// ---------------------------------------------------------------------------------------------
//  Copyright (c) 2021-2023, Jiaqi Liu. All rights reserved.
//  See LICENSE file in the project root for license information.
// ---------------------------------------------------------------------------------------------

namespace Pal3.Game.Scene.SceneObjects.Common
{
    using System;
    using Actor.Controllers;
    using Engine.Abstraction;
    using Engine.Extensions;
    using Engine.Services;
    using GamePlay;
    using UnityEngine;

    public class StandingPlatformController : GameEntityBase
    {
        public event EventHandler<GameObject> OnPlayerActorEntered;
        public event EventHandler<GameObject> OnPlayerActorExited;

        public int LayerIndex { get; private set; }

        private BoxCollider _collider;
        private Bounds _triggerBounds;
        private float _platformHeightOffset;

        private PlayerActorManager _playerActorManager;

        protected override void OnEnableGameEntity()
        {
            _playerActorManager = ServiceLocator.Instance.Get<PlayerActorManager>();
        }

        protected override void OnDisableGameEntity()
        {
            if (_collider != null)
            {
                _collider.Destroy();
                _collider = null;
            }
        }

        public void Init(Bounds triggerBounds, int layerIndex, float platformHeightOffset = 0f)
        {
            _triggerBounds = triggerBounds;
            LayerIndex = layerIndex;
            _platformHeightOffset = platformHeightOffset;

            if (_collider == null)
            {
                _collider = gameObject.AddComponent<BoxCollider>();
            }

            _collider.center = _triggerBounds.center;
            _collider.size = _triggerBounds.size;
            _collider.isTrigger = true;
        }

        public Collider GetCollider()
        {
            return _collider;
        }

        public float GetPlatformHeight()
        {
            // A little bit lower than the collider bounds just to make sure
            // the actor is always inside the collider when standing on the platform.
            return _collider.bounds.max.y + _platformHeightOffset - 0.05f;
        }

        private void OnTriggerEnter(Collider otherCollider)
        {
            if (otherCollider.gameObject.GetComponent<ActorController>() is { } actorController &&
                actorController.GetActor().Id == (int) _playerActorManager.GetPlayerActor())
            {
                OnPlayerActorEntered?.Invoke(this, otherCollider.gameObject);
            }
        }

        private void OnTriggerExit(Collider otherCollider)
        {
            if (otherCollider.gameObject.GetComponent<ActorController>() is { } actorController &&
                actorController.GetActor().Id == (int) _playerActorManager.GetPlayerActor())
            {
                OnPlayerActorExited?.Invoke(this, otherCollider.gameObject);
            }
        }
    }
}