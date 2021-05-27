﻿// <copyright file="Row.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Allors.Excel.Interop
{
    public class Row : IRow, IComparable<Row>
    {
        private bool hidden;

        public Row(Worksheet worksheet, int index)
        {
            Worksheet = worksheet;
            Index = index;
        }

        Excel.IWorksheet IRow.Worksheet => Worksheet;

        public Worksheet Worksheet { get; }

        public int Index { get; internal set; }

        bool IRow.Hidden { get => Hidden; set => Hidden = value; }

        public bool Hidden
        {
            get => hidden;
            set
            {
                hidden = value;
                Worksheet.AddDirtyRow(this);
            }
        }     

        public int CompareTo(Row other)
        {
            return Index.CompareTo(other.Index);
        }
    }
}
