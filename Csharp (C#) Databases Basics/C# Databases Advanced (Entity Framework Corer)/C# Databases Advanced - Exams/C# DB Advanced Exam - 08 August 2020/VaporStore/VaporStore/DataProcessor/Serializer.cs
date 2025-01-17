﻿namespace VaporStore.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using System;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Xml.Serialization;
    using VaporStore.DataProcessor.ExportDto;

    public static class Serializer
    {
        public static string ExportGamesByGenres(VaporStoreDbContext context, string[] genreNames)
        {
            var data = context.Genres.Where(x => genreNames.Contains(x.Name)).ToList()
                .Select(x => new
                {
                    Id = x.Id,
                    Genre = x.Name,
                    Games = x.Games.Select(g => new
                    {
                        Id = g.Id,
                        Title = g.Name,
                        Developer = g.Developer.Name,
                        Tags = string.Join(", ", g.GameTags.Select(gt => gt.Tag.Name)),
                        Players = g.Purchases.Count(),

                    })
                        .Where(g => g.Players > 0)
                        .OrderByDescending(g => g.Players)
                        .ThenBy(g => g.Id),
                    TotalPlayers = x.Games.Sum(g => g.Purchases.Count())
                })
                .OrderByDescending(x => x.TotalPlayers)
                .ThenBy(x => x.Id);

            return JsonConvert.SerializeObject(data, Formatting.Indented);
        }

        public static string ExportUserPurchasesByType(VaporStoreDbContext context, string purchaseType)
        {
            var data = context.Users.ToList()
                .Where(x => x.Cards.Any(c => c.Purchases.Any(p=> p.Type.ToString() == purchaseType)))
                .Select(x => new UserXmlExportModel
                {
                    Username = x.Username,
                    TotalSpent = x.Cards.Sum(
                          c => c.Purchases.Where(p => p.Type.ToString() == purchaseType)
                          .Sum(p => p.Game.Price)),
                    Purchases = x.Cards.SelectMany(c => c.Purchases)
                                        .Where(p => p.Type.ToString() == purchaseType)
                       .Select(p => new PurchaseXmlExportModel
                       {
                           Card = p.Card.Number,
                           Cvc = p.Card.Cvc,
                           Date = p.Date.ToString("yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture),
                           Game = new GameXmlExportModel
                           {
                               Title = p.Game.Name,
                               Price = p.Game.Price,
                               Genre = p.Game.Genre.Name,
                           },
                       })
                       .OrderBy(x => x.Date)
                       .ToArray()
                }).OrderByDescending(x => x.TotalSpent).ThenBy(x => x.Username).ToArray();

            XmlSerializer xmlSerializer = new XmlSerializer(typeof(UserXmlExportModel[]),
                new XmlRootAttribute("Users"));

            var sw = new StringWriter();

            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");

            xmlSerializer.Serialize(sw, data, ns);

            return sw.ToString();
        }
    }
}