﻿// <copyright file="Binding.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Allors.Excel
{
    public class Binding : IBinding
    {
        private readonly Action<ICell> toCell;

        private readonly Action<ICell> toDomain;

        public Binding(Action<ICell> toCell = null, Action<ICell> toDomain = null)
        {
            this.toCell = toCell;
            this.toDomain = toDomain;
        }

        public bool OneWayBinding => toDomain == null;

        public bool TwoWayBinding => !OneWayBinding;

        public object Value { get; }

        public void ToCell(ICell cell)
        {
            toCell?.Invoke(cell);
        }

        public void ToDomain(ICell cell)
        {
            toDomain?.Invoke(cell);
        }
    }
}