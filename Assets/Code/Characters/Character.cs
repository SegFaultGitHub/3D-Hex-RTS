using System;
using System.Collections.Generic;
using System.Threading;
using Code.Interactable;
using Code.Tiles;
using Code.UI;
using UnityEngine;

namespace Code.Characters {
    public class Character : Selectable, IInteractable, IWithWorldCanvas {
        protected const float TURN_SMOOTH_TIME = 0.1f;
        private static readonly Collider[] GROUND_COLLIDER = new Collider[1];
        private static readonly Vector3 GRAVITY = new(0, -0.02f, 0);
        private static readonly int MOVING = Animator.StringToHash("Moving");
        [field: SerializeField] protected Tile GroundTile;
        [field: SerializeField] private float Speed;

        [field: SerializeField] private Canvas Canvas;
        protected Animator Animator;
        private bool ComputingPath;
        protected CharacterController Controller;
        private Vector3 FallingSpeed;

        private Transform GroundCollider;
        private Vector3 MovementDirection;
        private LTDescr PopupTween;
        protected float TurnSmoothVelocity;
        [field: SerializeField] public Path Path { protected get; set; }
        [field: SerializeField] public ProgressiveBarUI UICreationPrefab { get; private set; }
        [field: SerializeField] public long CreationDuration { get; private set; }

        public float StepOffset => this.Controller.stepOffset;

        protected override void Awake() {
            base.Awake();
            this.Controller = this.GetComponent<CharacterController>();
            this.Animator = this.GetComponentInChildren<Animator>();
            this.Path = new Path {
                Complete = true,
                Tiles = new List<Tile>()
            };
            this.GroundCollider = this.transform.Find("Ground collider").transform;
            this.Canvas.worldCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            this.Canvas.gameObject.SetActive(false);
            this.Canvas.transform.localScale *= 0;
        }

        protected virtual void Update() {
            GROUND_COLLIDER[0] = null;
            Physics.OverlapSphereNonAlloc(this.GroundCollider.position, 0.01f, GROUND_COLLIDER);
            if (GROUND_COLLIDER[0] is not null)
                this.SetGroundTile(GROUND_COLLIDER[0].GetComponentInParent<Tile>());

            if (this.Path.Tiles.Count != 0) {
                this.Path.Tiles.Remove(this.GroundTile);
                if (this.Path.Tiles.Count != 0) {
                    if (!this.Path.Complete && this.Path.Tiles.Count < this.Path.RenewAt)
                        this.SetPath(this.Path.Destination);
                    Tile tileTarget = this.Path.Tiles[0];
                    Vector3 target = tileTarget.transform.position;
                    Vector3 position = this.transform.position;
                    if (tileTarget.Occupied && !tileTarget.AllowMultipleEntities)
                        this.SetPath(this.Path.Destination);
                    else
                        this.MovementDirection = new Vector3(target.x - position.x, 0, target.z - position.z);
                }
            } else if (this.Path.Complete) {
                Vector3 target = this.GroundTile.transform.position;
                Vector3 position = this.transform.position;
                this.MovementDirection = new Vector3(target.x - position.x, 0, target.z - position.z);
                if (this.MovementDirection.magnitude <= this.Speed)
                    this.MovementDirection *= 0;
            } else {
                this.SetPath(this.Path.Destination);
            }

            this.Animator.SetBool(MOVING, this.MovementDirection.sqrMagnitude != 0);

            ((IWithWorldCanvas)this).RotateCanvas(this.Canvas);
        }

        private void FixedUpdate() {
            #region Movement
            if (this.MovementDirection.sqrMagnitude != 0) {
                float yValue = this.MovementDirection.y;
                float targetAngle = Mathf.Atan2(this.MovementDirection.x, this.MovementDirection.z) * Mathf.Rad2Deg;
                float angle = Mathf.SmoothDampAngle(
                    this.transform.eulerAngles.y,
                    targetAngle,
                    ref this.TurnSmoothVelocity,
                    TURN_SMOOTH_TIME
                );
                this.transform.rotation = Quaternion.Euler(0, angle, 0);

                Vector3 movementDirection = (Quaternion.Euler(0, targetAngle, 0) * Vector3.forward).normalized;
                movementDirection.y = yValue;
                this.Controller.Move(this.Speed * movementDirection);
            }

            CollisionFlags flags = this.Controller.Move(this.FallingSpeed);
            if (flags == CollisionFlags.Below)
                this.FallingSpeed = GRAVITY;
            else
                this.FallingSpeed += GRAVITY;
            #endregion
        }

        public void Interact(Selectable selected) {
            throw new NotImplementedException();
        }

        public void SetGroundTile(Tile tile) {
            if (this.GroundTile is not null)
                this.GroundTile.Occupied = false;
            if (tile is not null)
                this.GroundTile = tile;
            if (this.GroundTile is not null)
                this.GroundTile.Occupied = true;
        }

        public virtual void GoToTile(Tile destination) {
            this.SetPath(destination);
        }

        protected void SetPath(Tile destination) {
            if (this.ComputingPath)
                return;
            this.ComputingPath = true;
            float offset = this.Controller.stepOffset;
            Thread thread = new(
                () => {
                    this.Path = this.GroundTile.PathFind(destination, offset);
                    // Debug.Log($"Found {(this.Path.Complete ? "" : "in")}complete path, {this.Path.Tiles.Count} tiles");
                    this.ComputingPath = false;
                }
            );
            thread.Start();
        }

        public void SetPosition(Vector3 position) {
            this.Controller.enabled = false;
            this.transform.position = position;
            // ReSharper disable once Unity.InefficientPropertyAccess
            this.Controller.enabled = true;
        }

        public override void MouseEnter() {
            this.ShowUI();
        }
        public override void MouseExit() {
            this.HideUI();
        }

        private void HideUI() {
            if (!this.Canvas.gameObject.activeSelf)
                return;

            if (this.PopupTween != null) LeanTween.cancel(this.PopupTween.id);

            this.PopupTween = LeanTween.scale(this.Canvas.gameObject, Vector3.zero, 0.25f)
                .setEaseInBack()
                .setOnComplete(
                    () => {
                        this.Canvas.gameObject.SetActive(false);
                        this.PopupTween = null;
                    }
                );
        }

        private void ShowUI() {
            if (this.PopupTween != null) LeanTween.cancel(this.PopupTween.id);

            this.Canvas.gameObject.SetActive(true);
            this.PopupTween = LeanTween.scale(this.Canvas.gameObject, Vector3.one, 0.25f)
                .setEaseOutBack()
                .setOnComplete(() => this.PopupTween = null);
        }
    }
}
