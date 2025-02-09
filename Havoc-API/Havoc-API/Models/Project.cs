using Havoc_API.Exceptions;
using Havoc_API.DTOs.Project;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Runtime.InteropServices;

namespace Havoc_API.Models
{
    public partial class Project
    {
        private string _name = null!;
        private string? _description;
        private DateTime? _start;
        private DateTime? _deadline;

        private static readonly byte[] Key = GenerateAesKey("YourSecretKey123456");
        private static readonly byte[] IV = Encoding.UTF8.GetBytes("YourSecretIV1234");

        public int ProjectId { get; private set; }

        public string Name
        {
            get => _name;
            private set
            {
                string trimmedValue = value.Trim();

                if (trimmedValue.Length > 25 || trimmedValue.Length == 0)
                    throw new StringLengthException(nameof(Name));

                _name = trimmedValue;
            }
        }

        public string? Description
        {
            get => _description;
            private set
            {
                if (value != null)
                {
                    string trimmedValue = value.Trim();

                    if (trimmedValue.Length > 200 || trimmedValue.Length == 0)
                        throw new StringLengthException(nameof(Description));

                    _description = trimmedValue;
                }
                _description = value;
            }
        }

        public byte[]? Background { get; private set; }

        public int CreatorId { get; private set; }

        public DateTime? Start
        {
            get => _start;
            private set
            {
                if (value.HasValue && Deadline.HasValue && value >= Deadline)
                    throw new WrongDateException("Start is after or equal to deadline");

                _start = value;
            }
        }

        public DateTime? Deadline
        {
            get => _deadline;
            private set
            {
                if (value < DateTime.Now)
                    throw new WrongDateException(nameof(Deadline) + ": " + value + "  Now: " + DateTime.Now);
                _deadline = value;
            }
        }

        public DateTime LastModified { get; private set; }

        public int ProjectStatusId { get; private set; }

        public virtual User Creator { get; private set; } = null!;

        public virtual ICollection<Participation> Participations { get; private set; } = new List<Participation>();

        public virtual ProjectStatus ProjectStatus { get; private set; } = null!;

        public virtual ICollection<Task> Tasks { get; private set; } = new List<Task>();

        private Project() { }

        public Project(string name, string? description, byte[]? background, DateTime? start, DateTime? deadline, User creator, ProjectStatus projectStatus)
        {
            if (start.HasValue && deadline.HasValue && start >= deadline)
                throw new WrongDateException("Start is after or equal to deadline");

            Name = name;
            Description = description;
            Background = background;
            CreatorId = creator.UserId;
            Start = start;
            Deadline = deadline;
            LastModified = DateTime.Now;
            ProjectStatusId = projectStatus.ProjectStatusId;
            ProjectStatus = projectStatus;
            Creator = creator;
        }

        public void UpdateProject(ProjectPATCH project)
        {
            if (!string.IsNullOrEmpty(project.Name))
            {
                Name = project.Name;
            }

            if (project.Description != null)
            {
                Description = project.Description;
            }

            if (project.StartDate.HasValue)
            {
                Start = project.StartDate;
            }

            if (project.Deadline.HasValue)
            {
                Deadline = project.Deadline;
            }

            LastModified = DateTime.Now;
        }

        public string GenerateInviteCode()
        {
            string rawString = $"{ProjectId}-{Name}";

            using Aes aes = Aes.Create();
            aes.Key = Key;
            aes.IV = IV;

            using MemoryStream ms = new();
            using (CryptoStream cs = new(ms, aes.CreateEncryptor(), CryptoStreamMode.Write))
            {
                using StreamWriter sw = new StreamWriter(cs);
                sw.Write(rawString);
            }
            return Convert.ToBase64String(ms.ToArray());
        }

        public static Dictionary<string, string> DecryptInviteCode(string inviteCode)
        {
            try
            {
                using Aes aes = Aes.Create();
                aes.Key = Key;
                aes.IV = IV;

                using MemoryStream ms = new(Convert.FromBase64String(inviteCode));
                using CryptoStream cs = new(ms, aes.CreateDecryptor(), CryptoStreamMode.Read);
                using StreamReader sr = new(cs);
                string decrypted = sr.ReadToEnd();
                string[] parts = decrypted.Split('-');

                if (parts.Length != 2)
                    throw new FormatException("Invalid invite code format");

                return new Dictionary<string, string>
                {
                    { "ProjectId", parts[0] },
                    { "ProjectName", parts[1] }
                };
            }
            catch (Exception)
            {
                throw new ArgumentException("Invalid invite code");
            }
        }

        private static byte[] GenerateAesKey(string secret)
        {
            using SHA256 sha256 = SHA256.Create();
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(secret));
        }
    }
}
