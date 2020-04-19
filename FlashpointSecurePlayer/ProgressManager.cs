﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static FlashpointSecurePlayer.Shared;
using static FlashpointSecurePlayer.Shared.Exceptions;

namespace FlashpointSecurePlayer {
    public static class ProgressManager {
        public const uint PBM_SETSTATE = 0x0410;
        public static readonly IntPtr PBST_NORMAL = (IntPtr)1;
        public static readonly IntPtr PBST_ERROR = (IntPtr)2;
        public static readonly IntPtr PBST_PAUSED = (IntPtr)3;

        private static ProgressBarStyle style = ProgressBarStyle.Marquee;
        private static int value = 0;
        private static IntPtr state = PBST_NORMAL;

        public static class CurrentGoal {
            private class Goal {
                private int size = 1;
                private int steps = 0;

                public int Size {
                    get {
                        return size;
                    }

                    set {
                        // new size cannot be less than old size
                        if (value < size) {
                            value = size;
                        }

                        // new size cannot be less than steps
                        if (value < Steps) {
                            value = Steps;
                        }

                        size = value;
                    }
                }

                public int Steps {
                    get {
                        return steps;
                    }

                    set {
                        // new steps cannot be less than old steps
                        if (value < steps) {
                            value = steps;
                        }

                        // new steps cannot be greater than size
                        if (value > Size) {
                            value = Size;
                        }

                        steps = value;
                    }
                }

                public Goal(int size = 1) {
                    Size = size;
                }
            }

            private static Stack<Goal> Goals = new Stack<Goal>();

            public static int Size {
                get {
                    if (!Goals.Any()) {
                        return 1;
                    }

                    return Goals.Peek().Size;
                }

                set {
                    if (!Goals.Any()) {
                        return;
                    }

                    Goals.Peek().Size = value;
                }
            }

            public static int Steps {
                get {
                    if (!Goals.Any()) {
                        return 0;
                    }
                    
                    return Goals.Peek().Steps;
                }

                set {
                    if (!Goals.Any()) {
                        return;
                    }

                    Goal goal = Goals.Peek();

                    if (value == goal.Size) {
                        // can't be true unless calling Stop
                        return;
                    }

                    goal.Steps = value;
                    Show();
                }
            }

            private static void Show() {
                Goal[] goalsArray = Goals.ToArray();

                if (goalsArray.ElementAtOrDefault(0) == null) {
                    return;
                }

                double multiplier = (double)goalsArray[0].Steps / goalsArray[0].Size;

                for (int i = 1;i < goalsArray.Length;i++) {
                    multiplier *= (double)(goalsArray[i].Steps + 1) / goalsArray[i].Size;
                }

                int progressManagerValue = (int)(multiplier * 100.0);

                if (progressManagerValue < ProgressManager.Value) {
                    return;
                }

                ProgressManager.Value = progressManagerValue;
            }

            public static void Start(int size = 1) {
                if (size > 0) {
                    Goals.Push(new Goal(size));
                }
            }

            public static void Stop() {
                if (!Goals.Any()) {
                    return;
                }

                Goal goal = Goals.Peek();
                goal.Steps = goal.Size;

                try {
                    Show();
                } finally {
                    Goals.Pop();
                }
            }
        }

        public static ProgressBar ProgressBar { get; set; } = null;

        private static ProgressBarStyle Style {
            get {
                return ProgressManager.style;
            }

            set {
                ProgressManager.style = value;

                if (ProgressBar == null) {
                    return;
                }

                ProgressBar.Style = ProgressManager.style;
            }
        }

        private static int Value {
            get {
                return ProgressManager.value;
            }

            set {
                ProgressManager.value = Math.Max(Math.Min(value, 100), 0);

                if (ProgressBar == null) {
                    return;
                }

                ProgressBar.Value = ProgressManager.value;
            }
        }

        private static IntPtr State {
            get {
                return ProgressManager.state;
            }

            set {
                ProgressManager.state = value;

                if (ProgressBar == null) {
                    return;
                }

                SendMessage(ProgressBar.Handle, PBM_SETSTATE, ProgressManager.state, IntPtr.Zero);
            }
        }

        public static void Reset() {
            Style = ProgressBarStyle.Blocks;
            Value = 0;

            Style = ProgressBarStyle.Continuous;
            State = PBST_NORMAL;
        }

        public static void ShowOutput() {
            Style = ProgressBarStyle.Continuous;
            State = PBST_NORMAL;
        }

        public static void ShowError() {
            Style = ProgressBarStyle.Continuous;
            Value = 100;

            State = PBST_ERROR;
        }
    }
}