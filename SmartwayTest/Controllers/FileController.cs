using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartwayTest.Data;
using SmartwayTest.Model;
using System;
using System.Reflection.Metadata.Ecma335;

namespace SmartwayTest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private readonly FileContext _context;

        public FileController(FileContext context)
        {
            _context = context;
        }

        [HttpPost("Upload")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Upload([FromForm] List<IFormFile> files, [FromQuery] int userId)
        {
            var group = new GroupModel
            {
                Id = Guid.NewGuid().ToString(),
                Name = string.Join(", ", files.Select(x => x.FileName)),
                UserId = userId,
                Files = files.Select(file => new FileModel
                {
                    Id = Guid.NewGuid().ToString(),
                    UserId = userId,
                    FileName = file.FileName
                }).ToList()
            };

            foreach (var file in group.Files)
                file.GroupModel = group;
            await _context.Groups.AddAsync(group);
            await _context.SaveChangesAsync();

            for (int i = 0; i < files.Count; i++)
            {
                var formFile = files[i];

                if (formFile.Length > 0)
                {
                    // Весь код ниже необходим только в случае подсчёта прогресса и его сохранения в базу данных.
                    using var memoryStream = new MemoryStream();
                    using (Stream fileStream = formFile.OpenReadStream())
                    {
                        byte[] buffer = new byte[16 * 1024];
                        int readBytes = 0;
                        long totalReadBytes = 0;

                        FileModel fileElement = group.Files.ElementAt(i);
                        while ((readBytes = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            await memoryStream.WriteAsync(buffer, 0, readBytes);
                            totalReadBytes += readBytes;
                            fileElement.FileContent = memoryStream.ToArray();
                            fileElement.Progress = (float)totalReadBytes / formFile.Length * 100;
                            await _context.SaveChangesAsync();
                        }
                        if (fileElement.Progress != 100)
                        {
                            fileElement.Progress = 100;
                            await _context.SaveChangesAsync();
                        }
                    }
                } else
                {
                    FileModel fileElement = group.Files.ElementAt(i);
                    fileElement.Progress = 100;
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(string.Join(", ", new string[] { group.Id }.Concat(group.Files.Select(f => f.Id))));
        }

        [HttpGet("Progress")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Progress(string id, [FromQuery] int userId)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null || (group.UserId != userId))
            {
                var file = await _context.Files.FindAsync(id);

                if (file == null || (file.UserId != userId))
                {
                    return Unauthorized();
                }

                return Ok(file.Progress);
            }
            return Ok(group.Progress);
        }

        [HttpGet("ListFiles")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListFiles([FromQuery] int userId)
        {
            var files = await _context.Files.Where(f => f.UserId == userId).ToListAsync();

            return Ok(files);
        }

        [HttpGet("ListGroups")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ListGroups([FromQuery] int userId)
        {
            var groups = await _context.Groups.Where(g => g.UserId == userId).ToListAsync();

            return Ok(groups);
        }

        [HttpGet("Download/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Download(string id, [FromQuery] int userId)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null || (group.UserId != userId))
            {
                var file = await _context.Files.FindAsync(id);

                if (file == null || (file.UserId != userId))
                {
                    return Unauthorized();
                }

                return File(file.FileContent, "application/octet-stream", file.FileName);
            }
            return new MultipartResult(group.Files.Select(file => new Model.MultipartContent() { ContentType = "application/octet-stream", FileName = file.FileName, Stream = new MemoryStream(file.FileContent) }).ToArray());
        }

        [HttpGet("GenerateLink/{id}")]
        public async Task<IActionResult> GenerateLink(string id, [FromQuery] int userId)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null || (group.UserId != userId))
            {
                var file = await _context.Files.FindAsync(id);

                if (file == null || (file.UserId != userId))
                {
                    return Unauthorized();
                }

                file.DownloadToken = Guid.NewGuid().ToString();

                await _context.SaveChangesAsync();

                return Ok($"https://localhost:5001/api/File/DownloadWithLink/{id}?token={file.DownloadToken}");
            }
            group.DownloadToken = Guid.NewGuid().ToString();

            await _context.SaveChangesAsync();

            return Ok($"https://localhost:5001/api/File/DownloadWithLink/{id}?token={group.DownloadToken}");
        }

        [HttpGet("DownloadWithLink/{id}")]
        public async Task<IActionResult> DownloadWithLink(string id, [FromQuery] string token)
        {
            var group = await _context.Groups.FindAsync(id);
            if (group == null || group.DownloadToken==null || group.DownloadToken != token)
            {
                var file = await _context.Files.FindAsync(id);

                if (file == null || file.DownloadToken == null || file.DownloadToken != token)
                {
                    return Unauthorized();
                }

                file.DownloadToken = null;
                await _context.SaveChangesAsync();

                return File(file.FileContent, "application/octet-stream", file.FileName);
            }

            group.DownloadToken = null;
            await _context.SaveChangesAsync();

            return new MultipartResult(group.Files.Select(file => new Model.MultipartContent() { ContentType = "application/octet-stream", FileName = file.FileName, Stream = new MemoryStream(file.FileContent) }).ToArray());
        }
    }
}