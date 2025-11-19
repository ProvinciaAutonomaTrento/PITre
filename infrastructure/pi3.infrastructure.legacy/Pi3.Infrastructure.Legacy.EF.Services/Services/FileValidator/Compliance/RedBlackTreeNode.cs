// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
using System;
using System.Collections.Generic;
using System.Text;

namespace Pi3.Infrastructure.Legacy.EF.Services.FileValidator.Compliance
{
    internal enum NodeColor { Red, Black }

    internal class RedBlackTreeNode<TItem> : BinaryTreeNodeBase<TItem, RedBlackTreeNode<TItem>>
    {
        public NodeColor Color;

        public RedBlackTreeNode()
        {
            Parent = Nil;
            Left = Nil;
            Right = Nil;
            Color = NodeColor.Black;
        }

        public static readonly RedBlackTreeNode<TItem> Nil = new RedBlackTreeNode<TItem>();
    }
}