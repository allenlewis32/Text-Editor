using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Text_Editor.Models
{
	public class FileModel
	{
		[Key]
		[Required]
		public string Name { get; set; }

		[Required]
		public string Content { get; set; }

		[Required]
		public string UserID { get; set; }

		[Required]
		[DisplayName("Last Accessed")]
		public DateTime LastAccessed { get; set; }
	}
}
