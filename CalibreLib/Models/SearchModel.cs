using CalibreLib.Models.Metadata;
using CalibreLib.Data;
using CalibreLib.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace CalibreLib.Models
{
    public class SearchModel
    {
        public string Title { get; set; }
        public List<int> Authors { get; set; } = new List<int>();
        public List<int> Publishers { get; set; } = new List<int>();
        public DateTime? PublishedDateFrom { get; set; }
        public DateTime? PublishedDateTo { get; set; }
        public int? ReadStatus { get; set; }
        public List<int> Tags { get; set; } = new List<int>();
        public List<int> Series { get; set; } = new List<int>();
        public List<int> Shelves { get; set; } = new List<int>();
        public List<int> Languages { get; set; } = new List<int>();
        public List<int> ExcludeTags { get; set; } = new List<int>();
        public List<int> ExcludeSeries { get; set; } = new List<int>();
        public List<int> ExcludeShelves { get; set; } = new List<int>();
        public List<int> ExcludeLanguages { get; set; } = new List<int>();
        public int? RatingAbove { get; set; }
        public int? RatingBelow { get; set; }
        public string Description { get; set; }


    }
}
