

namespace BloomFilterKata
{
    using System;
    using System.Collections;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;
    public enum HashAlgorithmType
    {
        MD5 = 0,
        SHA256 = 1,
        SHA512 = 2
    }
    
    public class BloomFilter
    {
        BitArray bitArray;
        int numHashes;
        int numEntriesToBeAdded;
        int numLoadedEntries;
        HashAlgorithm hash;

        // Defaults set for 64kbytes with number of words from Kata example
        public BloomFilter(int bitArraySize = 512000, int entriescount = 338883, HashAlgorithmType hat = HashAlgorithmType.MD5)
        {
            this.bitArray = new BitArray(bitArraySize);
            this.numEntriesToBeAdded = entriescount;
            this.hash = AssignHashAlgorithm(hat);
            this.numLoadedEntries = 0;
        }

        public HashAlgorithm AssignHashAlgorithm(HashAlgorithmType hat)
        {
            HashAlgorithm ha;
            switch (hat)
            {
                case HashAlgorithmType.SHA256:
                    ha = SHA256.Create();
                    break;
                case HashAlgorithmType.SHA512:
                    ha = SHA512.Create();
                    break;
                default:
                    ha = MD5.Create();
                    break;
            }

            return ha;
        }

        public bool IngestWords(string path, int numHashes = 0)
        {
            if (string.IsNullOrEmpty(path))
            {
                path = string.Format(@"wordlist.txt");
            }
            
            Console.WriteLine(string.Format("Ingest data from {0}", path));

            
            // set num hashes
            this.numHashes = numHashes == 0 ? GetOptimalHashSize() : numHashes;

            // read file contents line by line
            foreach (string word in File.ReadLines(path))
            {
                AddWord(word);
                this.numLoadedEntries++;
            }

            return true;
        }

        public void AddWord(string word)
        {
            // Console.WriteLine(string.Format("Ingest word {0}", word));

            int[] indexes = GetIndexesOffSetInHash(word);
            foreach (int index in indexes)
            {
                this.bitArray[index] = true;
            }

            // check set
            foreach (int index in indexes)
            {
                if (!this.bitArray[index])
                {
                    Console.WriteLine(string.Format("Index {0} is set, {1} may be present",  index, word));
                    throw new InvalidDataException(string.Format("Index {0} should be True, for word {1}",  index, word));
                }
                
                // Console.WriteLine(string.Format("Index {0} was indeed set for word {1}",  index, word));                    
            }
        }

        public bool IsWordInMap(string word)
        {
            int[] indexes = GetIndexesOffSetInHash(word);
            foreach (int index in indexes)
            {
                if (this.bitArray[index])
                {
                    // Console.WriteLine(string.Format("Index {0} is set, {1} may be present",  index, word));
                    return true;
                }
                // Console.WriteLine(string.Format("Index {0} is set to False, {1} may be present",  index, word));
            }

            Console.WriteLine(string.Format("{0} was not present", word));
            return false;
        }

        /// https://en.wikipedia.org/wiki/Bloom_filter#Optimal_number_of_hash_functions suggests optimal number of hashes as
        public int GetOptimalHashSize()
        { 
            int numHashes = Convert.ToInt32(((this.bitArray.Length * Math.Log(2)) / this.numEntriesToBeAdded));
            Console.WriteLine(string.Format("With m: {0} & n : {1}, optimal k(numHashes) will be set to {2}", this.bitArray.Length, this.numEntriesToBeAdded, numHashes));

            return numHashes;
        }

        /// Gets list of indexes using mulitple ComputeHash methods 
        private int[] GetIndexesOffSetInHash(string word)
        {
            int[] indexes = new int[this.numHashes];
            StringBuilder sBuilder = new StringBuilder();
            sBuilder.Append("[");
            for (int i = 0; i < this.numHashes; i++)
            {
                indexes[i] = GetIndexOffSetInHash(word, i);
                // Console.WriteLine(string.Format("Index {0} will be set to True for word {1}", indexes[i], word));
                sBuilder.Append(indexes[i] + ",");
            }
            sBuilder.Append("]");
            // Console.WriteLine(string.Format("'{0}' indexes were calculated", sBuilder.ToString()));

            return indexes;
        }

        private int GetIndexOffSetInHash(string word, int offset)
        {
            byte[] data = this.hash.ComputeHash(Encoding.UTF8.GetBytes(word + offset));
            int index = Math.Abs(BitConverter.ToInt32(data, 0) % this.bitArray.Length);

            return index;
        }

        /// Gets list of hash using a single ComputeHash method and  applying offset on bitconversion
        private int[] GetIndexesOffSetInBitConverter(string word)
        {
            int[] indexes = new int[this.numHashes];
            byte[] data = this.hash.ComputeHash(Encoding.UTF8.GetBytes(word));
            for (int i = 0; i < this.numHashes; i++)
            {
                int index = Math.Abs(BitConverter.ToInt32(data, i) % this.bitArray.Length);
                indexes[i] = index;
                // Console.WriteLine(string.Format("Index {0} will be set to True for word {1}", index, word));
            }

            return indexes;
        }

        private int GetIndexOffSetInBitConverter(string word, int offset)
        {
            byte[] data = this.hash.ComputeHash(Encoding.UTF8.GetBytes(word));
            int index = Math.Abs(BitConverter.ToInt32(data, offset) % this.bitArray.Length);

            return index;
        }
    }
}
