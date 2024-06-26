﻿// <copyright file="Workbook.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Excel.Headless
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml;

    public class Workbook(AddIn addIn) : IWorkbook
    {
        private int counter;


      

        public AddIn AddIn { get; } = addIn;

        public List<Worksheet> WorksheetList { get; set; } = new();

        public IWorksheet[] Worksheets => this.WorksheetList.Cast<IWorksheet>().ToArray();

        public bool IsActive { get; private set; }

        public Dictionary<string, Range> NamedRangeByName { get; } = new Dictionary<string, Range>();

        public event EventHandler<Allors.Excel.Hyperlink> OnHyperlinkClicked;

        public IWorksheet AddWorksheet(int? index = null, IWorksheet before = null, IWorksheet after = null)
        {
            var worksheet = new Worksheet(this)
            {
                Name = $"Sheet{++this.counter}"
            };

            if (index != null)
            {
                if (index == 0)
                {
                    var active = this.WorksheetList.FirstOrDefault(v => v.IsActive);
                    this.WorksheetList.Insert(this.WorksheetList.IndexOf(active), worksheet);
                }
                else
                {
                    if (index.Value > this.WorksheetList.Count)
                    {
                        this.WorksheetList.Add(worksheet);
                    }
                    else
                    {
                        this.WorksheetList.Insert(index.Value - 1, worksheet);
                    }
                }
            }
            else if (before != null)
            {
                this.WorksheetList.Insert(this.WorksheetList.IndexOf(before as Worksheet), worksheet);
            }
            else if (after != null)
            {
                this.WorksheetList.Insert(this.WorksheetList.IndexOf(after as Worksheet) + 1, worksheet);
            }
            else
            {
                this.WorksheetList.Add(worksheet);
            }

            worksheet.Activate();

            this.AddIn.Program.OnNew(worksheet).ConfigureAwait(false);

            return worksheet;
        }

        public void Close(bool? saveChanges = null, string fileName = null) => this.AddIn.Remove(this);

        public void Activate()
        {
            foreach (var workbook in this.AddIn.WorkbookList)
            {
                workbook.IsActive = false;
            }

            this.IsActive = true;
        }

        public Range[] GetNamedRanges(string refersToSheetName = null) => this.NamedRangeByName.Values.ToArray();

        public IWorksheet Copy(IWorksheet source, IWorksheet beforeWorksheet) => throw new NotImplementedException();

        public void SetNamedRange(string name, Range range)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }

            if (range == null)
            {
                throw new ArgumentNullException(nameof(range));
            }

            this.NamedRangeByName[name] = range;
            range.Name = name;
        }

        public IBuiltinProperties BuiltinProperties { get; } = new BuiltinProperties();

        public ICustomProperties CustomProperties { get; } = new CustomProperties();

        private string customXml;
        private Dictionary<string, XmlDocument> CustomXmlParts = new Dictionary<string, XmlDocument>();
        public string SetCustomXml(XmlDocument xmlDocument)
        {
            var id = Guid.NewGuid().ToString(); // Generate a unique ID for the custom XML part

            this.CustomXmlParts[id] = xmlDocument; // Store the XML document in the dictionary

            return id; // Return the ID of the custom XML part
        }
        
        public IWorksheet[] WorksheetsByIndex => this.Worksheets;

        public XmlDocument GetCustomXmlById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID cannot be null or empty.", nameof(id));
            }

            if (this.CustomXmlParts.TryGetValue(id, out var xmlDocument))
            {
                return xmlDocument;
            }

            return null;
        }

        public bool TryDeleteCustomXmlById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentException("ID cannot be null or empty.", nameof(id));
            }

            // Check if the custom XML part exists in the dictionary
            if (this.CustomXmlParts.ContainsKey(id))
            {
                // Remove the custom XML part from the dictionary
                this.CustomXmlParts.Remove(id);
                return true;
            }

            return false;
        }

        public bool TrySetCustomProperty(string name, dynamic value) => throw new NotImplementedException();

        public void HyperlinkClicked(Allors.Excel.Hyperlink hyperlink) => throw new NotImplementedException();
    }
}
