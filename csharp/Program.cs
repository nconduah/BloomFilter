
namespace BloomFilterKata
{
    using System;

    class Program
    {
        static void Main(string[] args)
        {            
            TestBasicWords(0); // optimal number of hashes
            TestBasicWords(10); // set number of hashes

            // TestWIthinputWords(); // optimal hashes mds5
            TestWIthinputWords(HashAlgorithmType.SHA256); // optimal hashes sha256
            // TestWIthinputWords(HashAlgorithmType.SHA512); // optimal hashes sha512
        }

        static void TestBasicWords(int numHashes)
        {
            BloomFilter bloom = new BloomFilter(1000, 7);
            bloom.IngestWords("wordlist_test.txt", numHashes);

            string wordtest = "test";
            Console.WriteLine(string.Format("*****'{0}' is present {1}", wordtest, bloom.IsWordInMap(wordtest)));

            wordtest = "ABC";
            Console.WriteLine(string.Format("'{0}' is present {1}", wordtest, bloom.IsWordInMap(wordtest)));

            wordtest = "ABM";
            Console.WriteLine(string.Format("'{0}' is present {1}", wordtest, bloom.IsWordInMap(wordtest)));

            wordtest = "to";
            Console.WriteLine(string.Format("'{0}' is present {1}", wordtest, bloom.IsWordInMap(wordtest)));

            wordtest = "zed";
            Console.WriteLine(string.Format("'{0}' is present {1}", wordtest, bloom.IsWordInMap(wordtest)));
        }

        static void TestWIthinputWords(HashAlgorithmType ha = HashAlgorithmType.MD5)
        {
            BloomFilter bloom = new BloomFilter(hat: ha);
            Console.WriteLine(string.Format("'{0}' is optimal number of hashes", bloom.GetOptimalHashSize()));

            bloom.IngestWords(string.Empty, 0);

            string wordtest = "cdknjvfdjk"; // false positive for md5
            Console.WriteLine(string.Format("'{0}' is present {1}", wordtest, bloom.IsWordInMap(wordtest)));

            wordtest = "chapless"; // true positive for md5
            Console.WriteLine(string.Format("'{0}' is present {1}", wordtest, bloom.IsWordInMap(wordtest)));

            wordtest = "throughput"; //true positive for md5
            Console.WriteLine(string.Format("'{0}' is present {1}", wordtest, bloom.IsWordInMap(wordtest)));

            wordtest = "tobaccox";// true negative for md5
            Console.WriteLine(string.Format("'{0}' is present {1}", wordtest, bloom.IsWordInMap(wordtest)));

            wordtest = "zedn"; // false positive for md5
            Console.WriteLine(string.Format("'{0}' is present {1}", wordtest, bloom.IsWordInMap(wordtest)));
        }
    }
}
