﻿namespace BisleriumServer.Model.DTO
{
    public class BlogModel
    {
        public string? Title { get; set; }
        public string? Description { get; set; }

		public IFormFile Image { get; set; }
	}
}
