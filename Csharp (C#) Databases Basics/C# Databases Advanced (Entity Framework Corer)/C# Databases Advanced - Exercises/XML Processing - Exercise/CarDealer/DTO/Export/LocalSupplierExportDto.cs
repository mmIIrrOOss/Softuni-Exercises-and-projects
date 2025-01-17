﻿using System.Xml.Serialization;

namespace CarDealer.DTO.Export
{

    [XmlType("supplier")]
    public class LocalSupplierExportDto
    {
        [XmlAttribute("id")]
        public int Id { get; set; }

        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlAttribute("parts-count")]
        public int PartsCount { get; set; }
    }
}
