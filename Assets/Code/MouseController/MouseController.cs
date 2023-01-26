using System;
using System.Collections.Generic;
using Code.Camera;
using Code.Characters;
using Code.Interactable;
using Code.Tiles;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Code.MouseController {
    public class MouseController : MonoBehaviour {
        private static readonly RaycastHit[] RAYCAST_HITS = new RaycastHit[10];
        [SerializeField] private LayerMask MouseCollisionLayer;
        [SerializeField] private LayerMask TileLayer;

        private UnityEngine.Camera Camera;
        private Vector2 CameraAngleVelocity;
        private CameraController CameraController;
        private Vector3 CameraVelocity;

        private Tile Castle;

        private Selectable MouseOver;

        private bool MouseOverUI;
        private Vector2 MousePosition;
        private Vector2 MousePositionDelta;
        private Selectable Selected;

        private float TargetCameraFOV;

        private LTDescr CameraFocusTween;
        private Vector3 CameraFocusPosition;

        private Builder PhantomBuildingBuilder;
        private LTDescr MovePhantomBuildingTween;
        private LTDescr EnablePhantomBuildingTween;
        private bool PhantomBuildingEnabled;
        [field: SerializeField] private PhantomBuilding PhantomBuilding;

        private bool CameraMoving => this.Input.MoveCameraInProgress || this.Input.RotateCameraInProgress;

        private void Start() {
            this.Input = new _Input();
            this.Camera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<UnityEngine.Camera>();
            this.CameraController = this.Camera.GetComponent<CameraController>();
            this.TargetCameraFOV = this.Camera.fieldOfView;
            this.Castle = GameObject.FindGameObjectWithTag("Castle").GetComponentInParent<Tile>();
        }

        private void Update() {
            this.SetMouseOverUI();
            this.GatherInput();
            this.HandleInput();

            #region Local functions
            void _MoveCamera() {
                if (this.CameraVelocity.sqrMagnitude == 0)
                    return;
                if (this.CameraFocusTween != null) {
                    LeanTween.cancel(this.CameraFocusTween.id);
                    this.CameraFocusTween = null;
                }
                this.Camera.transform.position += this.CameraVelocity;
                this.CameraVelocity *= 0.95f;
                if (this.CameraVelocity.magnitude < 0.001f)
                    this.CameraVelocity *= 0;
            }
            void _RotateCamera() {
                if (this.CameraAngleVelocity.sqrMagnitude == 0)
                    return;
                if (this.CameraFocusTween != null) {
                    LeanTween.cancel(this.CameraFocusTween.id);
                    this.CameraFocusTween = null;
                }
                Vector3 angles = this.Camera.transform.eulerAngles;
                angles.y += this.CameraAngleVelocity.x;
                angles.x -= this.CameraAngleVelocity.y;
                angles.x = Mathf.Clamp(angles.x, 25, 55);
                this.Camera.transform.eulerAngles = angles;
                this.CameraAngleVelocity *= 0.95f;
                if (this.CameraAngleVelocity.magnitude < 0.001f)
                    this.CameraAngleVelocity *= 0;
            }
            void _Zoom() {
                if (this.Input.ScrollValue != 0)
                    this.TargetCameraFOV = Mathf.Clamp(this.TargetCameraFOV - Mathf.Sign(this.Input.ScrollValue) * 5, 30, 75);
                if (Mathf.Abs(this.TargetCameraFOV - this.Camera.fieldOfView) > 1f)
                    this.Camera.fieldOfView += Mathf.Sign(this.TargetCameraFOV - this.Camera.fieldOfView) * 1f;
                else
                    this.Camera.fieldOfView = this.TargetCameraFOV;
            }
            void _CheckSelectableUnderMouse() {
                if (this.CameraMoving) return;

                Selectable previous = this.MouseOver;
                this.MouseOver = this.Raycast<Selectable>(this.MouseCollisionLayer);

                if (this.MouseOver == previous)
                    return;
                if (this.MouseOver is not null && this.MouseOver != this.Selected) {
                    this.MouseOver.Select();
                    this.MouseOver.MouseEnter();
                }
                // ReSharper disable once InvertIf
                if (previous is not null && previous != this.Selected) {
                    previous.Deselect();
                    previous.MouseExit();
                }
            }
            void _ShowPhantomBuilding(Vector3 position) {
                if (this.PhantomBuildingEnabled)
                    return;
                this.PhantomBuildingEnabled = true;
                if (this.EnablePhantomBuildingTween == null) this.PhantomBuilding.transform.position = position;
                if (this.EnablePhantomBuildingTween != null) LeanTween.cancel(this.EnablePhantomBuildingTween.id);
                this.EnablePhantomBuildingTween = LeanTween.scale(this.PhantomBuilding.gameObject, Vector3.one, 0.3f)
                    .setEaseOutBack()
                    .setOnComplete(() => this.EnablePhantomBuildingTween = null);
            }
            void _PlacePhantomBuilding() {
                if (this.CameraMoving) return;
                if (this.PhantomBuilding is null) return;
                if (this.MouseOver is not null) {
                    this.HidePhantomBuilding();
                    return;
                }

                Tile tile = this.GetTileUnderMouse();
                if (tile is null || !tile.Walkable || !tile.Seen || tile.Occupied) {
                    this.HidePhantomBuilding();
                    return;
                }

                Vector3 position = tile.transform.position;
                position.y = tile.Height;
                _ShowPhantomBuilding(position);

                if (this.PhantomBuilding.GridPosition == tile.GridPosition)
                    return;
                if (this.MovePhantomBuildingTween != null) LeanTween.cancel(this.MovePhantomBuildingTween.id);
                this.MovePhantomBuildingTween = LeanTween.move(this.PhantomBuilding.gameObject, position, 0.05f)
                    .setEaseOutQuad()
                    .setOnComplete(() => this.MovePhantomBuildingTween = null);
                this.PhantomBuilding.GridPosition = tile.GridPosition;
                this.PhantomBuilding.Tile = tile;
            }
            #endregion

            _MoveCamera();
            _RotateCamera();
            _Zoom();
            _CheckSelectableUnderMouse();
            _PlacePhantomBuilding();
        }

        public void CreatePhantomBuilding(Builder builder, PhantomBuilding phantomBuilding) {
            this.PhantomBuildingBuilder = builder;
            this.PhantomBuilding = Instantiate(phantomBuilding);
            Vector3 angles = new(0, Random.Range(0, 360), 0);
            this.PhantomBuilding.transform.eulerAngles = angles;
        }

        private void HidePhantomBuilding() {
            if (!this.PhantomBuildingEnabled)
                return;
            this.PhantomBuildingEnabled = false;
            if (this.EnablePhantomBuildingTween != null) LeanTween.cancel(this.EnablePhantomBuildingTween.id);
            this.EnablePhantomBuildingTween = LeanTween.scale(this.PhantomBuilding.gameObject, Vector3.zero, 0.3f)
                .setEaseInBack()
                .setOnComplete(() => this.EnablePhantomBuildingTween = null);
        }

        private void MoveCamera() {
            Vector2 delta = Quaternion.Euler(0, 0, -this.Camera.transform.eulerAngles.y) * this.MousePositionDelta / 60;
            this.CameraVelocity = new Vector3(-delta.x, 0, -delta.y);
        }

        private void RotateCamera() {
            this.CameraAngleVelocity = -this.MousePositionDelta / 30;
        }

        private void Select() {
            if (this.PhantomBuilding is not null && this.PhantomBuildingEnabled) {
                Building building = this.PhantomBuilding.Build();
                this.PhantomBuilding = null;
                this.PhantomBuildingBuilder.InteractWith(building);
                this.PhantomBuildingBuilder = null;
                return;
            }

            switch (this.Selected) {
                case null:
                    switch (this.MouseOver) {
                        case Player mouseOver:
                            this.SelectEntity(mouseOver);
                            break;
                        case Building mouseOver:
                            mouseOver.Interact(this.Selected);
                            this.SelectEntity(mouseOver);
                            break;
                        case null:
                            this.Deselect();
                            break;
                    }
                    break;
                case Player selected:
                    switch (this.MouseOver) {
                        case Player mouseOver:
                            this.SelectEntity(mouseOver);
                            break;
                        case Building mouseOver:
                            mouseOver.Interact(this.Selected);
                            this.SelectEntity(mouseOver);
                            break;
                        case null:
                            Tile tile = this.GetTileUnderMouse();
                            if (tile is null) break;
                            tile.Feedback();
                            selected.GoToTile(tile);
                            break;
                    }
                    break;
                case Building selected:
                    switch (this.MouseOver) {
                        case Player mouseOver:
                            this.SelectEntity(mouseOver);
                            break;
                        case Building mouseOver:
                            mouseOver.Interact(this.Selected);
                            this.SelectEntity(mouseOver);
                            break;
                        case null:
                            this.Deselect();
                            break;
                    }
                    break;
            }
        }

        private void Interact() {
            if (this.PhantomBuilding is not null) {
                this.HidePhantomBuilding();
                this.EnablePhantomBuildingTween.setDestroyOnComplete(true);
                this.PhantomBuilding = null;
                this.PhantomBuildingBuilder = null;
                return;
            }

            switch (this.Selected) {
                case null:
                    break;
                case Player selected:
                    switch (this.MouseOver) {
                        case Player mouseOver:
                            break;
                        case Building mouseOver:
                            selected.InteractWith(mouseOver);
                            break;
                        case null:
                            this.Deselect();
                            break;
                    }
                    break;
                case Building selected:
                    switch (this.MouseOver) {
                        case Player mouseOver:
                            break;
                        case Building mouseOver:
                            break;
                        case null:
                            this.Deselect();
                            break;
                    }
                    break;
            }
        }

        public void Deselect(Selectable selected) {
            if (this.Selected != selected)
                return;
            this.Deselect();
        }

        private void Deselect() {
            if (this.Selected is null)
                return;
            this.Selected.Deselect();
            this.Selected.MouseExit();
            if (this.Selected is Player player)
                player.OnDeselect();
            this.Selected = null;
        }

        private void SelectEntity(Selectable selectable) {
            if (this.Selected == selectable)
                return;
            this.Deselect();
            this.Selected = selectable;
            this.Selected.Select();
            if (this.Selected is Player player)
                player.OnSelect();
        }

        private void FocusOn(Vector3 position) {
            this.CameraVelocity *= 0;
            this.CameraAngleVelocity *= 0;

            if (this.CameraFocusTween != null) {
                if (position != this.CameraFocusPosition) LeanTween.cancel(this.CameraFocusTween.id);
                else return;
            }

            this.CameraFocusPosition = position;
            this.CameraFocusTween = this.CameraController.LookAt(position).setOnComplete(() => this.CameraFocusTween = null);
        }

        private Tile GetTileUnderMouse() {
            return this.Raycast<Tile>(this.TileLayer);
        }

        private void SetMouseOverUI() {
            if (this.Input.MoveCameraPerformed
                || this.Input.RotateCameraPerformed
                || this.Input.MoveCameraInProgress
                || this.Input.RotateCameraInProgress) {
                this.MouseOverUI = false;
                return;
            }

            PointerEventData eventData = new(EventSystem.current) {
                position = this.MousePosition
            };
            List<RaycastResult> uiRaycasts = new();
            EventSystem.current.RaycastAll(eventData, uiRaycasts);

            this.MouseOverUI = uiRaycasts.Count > 0;
        }

        private T Raycast<T>(LayerMask layer) {
            if (this.MouseOverUI) return default;

            for (int i = 0; i < RAYCAST_HITS.Length; i++) RAYCAST_HITS[i] = default;
            Physics.RaycastNonAlloc(this.Camera.ScreenPointToRay(this.MousePosition), RAYCAST_HITS, 1000, layer);

            float minDistance = float.MaxValue;
            T result = default;
            foreach (RaycastHit hit in RAYCAST_HITS) {
                if (hit.collider is null)
                    return result;
                if (minDistance < hit.distance)
                    continue;
                minDistance = hit.distance;
                T current = hit.collider.GetComponentInParent<T>();
                if (current is not null) result = current;
            }
            return result;
        }

        #region Input
        private InputActions InputActions;

        [Serializable]
        // ReSharper disable once InconsistentNaming
        private class _Input {
            public bool MoveCameraPerformed;
            public bool MoveCameraInProgress;
            public bool MoveCameraEnded;
            // --
            public bool RotateCameraPerformed;
            public bool RotateCameraInProgress;
            public bool RotateCameraEnded;
            // --
            public bool InteractPerformed;
            // --
            [FormerlySerializedAs("DeselectPerformed")]
            public bool SelectPerformed;
            // --
            public int ScrollValue;
            // --
            public bool FocusOnCastlePerformed;
            // --
            public bool FocusOnSelectedInProgress;
        }
        private _Input Input;

        private void OnEnable() {
            this.InputActions = new InputActions();
            this.InputActions.Camera.Enable();
            this.InputActions.Globals.Enable();
            this.InputActions.Units.Enable();
        }

        private void OnDisable() {
            this.InputActions.Camera.Disable();
            this.InputActions.Globals.Disable();
            this.InputActions.Units.Disable();
        }

        private void GatherInput() {
            this.Input = new _Input {
                MoveCameraPerformed = !this.MouseOverUI && this.InputActions.Camera.MoveCamera.WasPerformedThisFrame(),
                MoveCameraInProgress = !this.MouseOverUI && this.Input.MoveCameraInProgress,
                MoveCameraEnded = !this.MouseOverUI && this.InputActions.Camera.MoveCamera.WasReleasedThisFrame(),
                // --
                RotateCameraPerformed = !this.MouseOverUI && this.InputActions.Camera.RotateCamera.WasPerformedThisFrame(),
                RotateCameraInProgress = !this.MouseOverUI && this.Input.RotateCameraInProgress,
                RotateCameraEnded = !this.MouseOverUI && this.InputActions.Camera.RotateCamera.WasReleasedThisFrame(),
                // --
                InteractPerformed = !this.MouseOverUI && this.InputActions.Units.Interact.WasPerformedThisFrame(),
                // --
                SelectPerformed = !this.MouseOverUI && this.InputActions.Units.Select.WasPerformedThisFrame(),
                // --
                ScrollValue = (int)this.InputActions.Camera.ZoomCamera.ReadValue<Vector2>().y,
                // --
                FocusOnCastlePerformed = this.InputActions.Camera.FocusOnCastle.WasPerformedThisFrame(),
                // --
                FocusOnSelectedInProgress = this.InputActions.Camera.FocusOnSelected.IsPressed()
            };

            if (this.Input.MoveCameraPerformed)
                this.Input.MoveCameraInProgress = true;
            if (this.Input.MoveCameraEnded)
                this.Input.MoveCameraInProgress = false;

            if (this.Input.RotateCameraPerformed)
                this.Input.RotateCameraInProgress = true;
            if (this.Input.RotateCameraEnded)
                this.Input.RotateCameraInProgress = false;
        }

        private void HandleInput() {
            this.MousePositionDelta = this.MousePosition;
            this.MousePosition = this.InputActions.Globals.MousePosition.ReadValue<Vector2>();
            this.MousePositionDelta = this.MousePosition - this.MousePositionDelta;

            if (this.Input.MoveCameraInProgress)
                this.MoveCamera();
            else if (this.Input.RotateCameraInProgress)
                this.RotateCamera();

            if (this.Input.InteractPerformed)
                this.Interact();

            if (this.Input.SelectPerformed)
                this.Select();

            if (this.Input.FocusOnCastlePerformed)
                this.FocusOn(this.Castle.transform.position + new Vector3(0, this.Castle.Height, 0));
            else if (this.Input.FocusOnSelectedInProgress && this.Selected is not null)
                this.FocusOn(this.Selected.transform.position);
        }
        #endregion
    }
}
