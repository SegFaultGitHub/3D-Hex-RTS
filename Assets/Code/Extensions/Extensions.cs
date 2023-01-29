﻿using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Code.Extensions {
    public static class MonoBehaviourExtensions {
        // Runs the Callback at the end of the current frame, after GUI rendering
        public static Coroutine OnEndOfFrame(this MonoBehaviour self, UnityAction Callback) {
            return self.StartCoroutine(EndOfFrameCoroutine(Callback));
        }

        public static IEnumerator EndOfFrameCoroutine(UnityAction Callback) {
            yield return new WaitForEndOfFrame();
            Callback?.Invoke();
        }

        // Runs the Callback after the next Update completes
        public static Coroutine OnUpdate(this MonoBehaviour self, UnityAction Callback) {
            return self.InUpdates(1, Callback);
        }

        // Runs the Callback after a given number of Updates complete
        public static Coroutine InUpdates(this MonoBehaviour self, int updates, UnityAction Callback) {
            return self.StartCoroutine(InUpdatesCoroutine(updates, Callback));
        }

        public static IEnumerator InUpdatesCoroutine(int updates, UnityAction Callback) {
            for (int i = 0; i < updates; i++) {
                yield return null;
            }
            Callback?.Invoke();
        }

        // Runs the Callback after the next FixedUpdate completes
        public static Coroutine OnFixedUpdate(this MonoBehaviour self, UnityAction Callback) {
            return self.InFixedUpdates(1, Callback);
        }

        // Runs the Callback after a given number of FixedUpdates complete
        public static Coroutine InFixedUpdates(this MonoBehaviour self, int ticks, UnityAction Callback) {
            return self.StartCoroutine(InFixedUpdatesCoroutine(ticks, Callback));
        }

        public static IEnumerator InFixedUpdatesCoroutine(int ticks, UnityAction Callback) {
            for (int i = 0; i < ticks; i++) {
                yield return new WaitForFixedUpdate();
            }
            Callback?.Invoke();
        }

        // Runs the Callback after a given number of seconds, after the Update completes
        public static Coroutine InSeconds(this MonoBehaviour self, float seconds, UnityAction Callback) {
            return self.StartCoroutine(InSecondsCoroutine(seconds, Callback));
        }

        private static IEnumerator InSecondsCoroutine(float seconds, UnityAction Callback) {
            yield return new WaitForSeconds(seconds);
            Callback?.Invoke();
        }
    }
}
