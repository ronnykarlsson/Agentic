using Agentic.Utilities;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Agentic.Embeddings.Cache
{
    public class FileEmbeddingCache : IEmbeddingCache
    {
        private readonly string _cacheDirectory;
        private readonly string _modelCacheDirectory;
        private readonly string _sessionName;
        private readonly object _lock = new object();

        public FileEmbeddingCache(string cacheDirectory, string sessionName)
        {
            _cacheDirectory = cacheDirectory ?? throw new ArgumentNullException(nameof(cacheDirectory));
            _sessionName = sessionName ?? throw new ArgumentNullException(nameof(sessionName));

            _cacheDirectory = FilePathResolver.ResolvePath(_cacheDirectory);
            _modelCacheDirectory = Path.Combine(_cacheDirectory, _sessionName);
            Directory.CreateDirectory(_modelCacheDirectory);
        }

        public bool TryGetEmbedding(string text, out float[] embedding)
        {
            string filePath = GetEmbeddingFilePath(text);
            if (File.Exists(filePath))
            {
                embedding = LoadEmbeddingFromFile(filePath);
                return true;
            }

            embedding = null;
            return false;
        }

        public void SaveEmbedding(string text, float[] embedding)
        {
            string filePath = GetEmbeddingFilePath(text);
            lock (_lock)
            {
                if (!File.Exists(filePath))
                {
                    SaveEmbeddingToFile(filePath, embedding);
                }
            }
        }

        private string GetEmbeddingFilePath(string text)
        {
            string fileName = $"{ComputeHash(text)}.emb";
            return Path.Combine(_modelCacheDirectory, fileName);
        }

        private void SaveEmbeddingToFile(string filePath, float[] embedding)
        {
            using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                using (var bw = new BinaryWriter(fs))
                {
                    bw.Write(embedding.Length);
                    foreach (var value in embedding)
                    {
                        bw.Write(value);
                    }
                }
            }
        }

        private float[] LoadEmbeddingFromFile(string filePath)
        {
            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var br = new BinaryReader(fs))
                {
                    int length = br.ReadInt32();
                    float[] embedding = new float[length];
                    for (int i = 0; i < length; i++)
                    {
                        embedding[i] = br.ReadSingle();
                    }
                    return embedding;
                }
            }
        }

        private string ComputeHash(string text)
        {
            using (var sha = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(text);
                byte[] hashBytes = sha.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}
