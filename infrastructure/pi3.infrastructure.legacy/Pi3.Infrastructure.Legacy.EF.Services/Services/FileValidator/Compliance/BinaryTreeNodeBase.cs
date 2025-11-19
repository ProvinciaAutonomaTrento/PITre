// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;

namespace Pi3.Infrastructure.Legacy.EF.Services.FileValidator.Compliance
{
    internal class BinaryTreeNodeBase<TItem, TTreeNode>
        where TTreeNode : BinaryTreeNodeBase<TItem, TTreeNode>
    {
        public TItem Data;

        public TTreeNode Parent;

        public TTreeNode Left;

        public TTreeNode Right;

        public bool IsLeftChild
        {
            get { return this == Parent.Left; }
        }

        public bool IsRightChild
        {
            get { return this == Parent.Right; }
        }
    }
}