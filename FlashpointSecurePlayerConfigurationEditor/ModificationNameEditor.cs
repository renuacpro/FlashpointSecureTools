﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FlashpointSecurePlayerConfigurationEditor {
    public partial class ModificationNameEditor : Form {
        public ModificationNameEditor() {
            InitializeComponent();
        }

        private void okButton_Click(object sender, EventArgs e) {
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e) {
            Close();
        }
    }
}