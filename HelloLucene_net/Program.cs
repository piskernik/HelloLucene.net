using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lucene;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Analysis;
using Lucene.Net.Util;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.QueryParsers.Classic;

namespace HelloLucene.net
{
    class Program
    {
        static void Main(string[] args)
        {
            Analyzer analyzer = new StandardAnalyzer(LuceneVersion.LUCENE_48);

            Directory index = new RAMDirectory();

            IndexWriterConfig config = new IndexWriterConfig(LuceneVersion.LUCENE_48, analyzer);

            IndexWriter w = new IndexWriter(index, config);
            addDoc(w, "Lucene in Action", "193398817");
            addDoc(w, "Lucene for Dummies", "55320055Z");
            addDoc(w, "Managing Gigabytes", "55063554A");
            addDoc(w, "The Art of Computer Science", "9900333X");
            w.Commit();

            string querystr = args.Length > 0 ? args[0] : "lucene";

            QueryParser parser = new QueryParser(LuceneVersion.LUCENE_48, "title", analyzer);
            Query query = parser.Parse(querystr);

            int hitsPerPage = 10;
            IndexReader reader = DirectoryReader.Open(index);
            IndexSearcher searcher = new IndexSearcher(reader);
            TopDocs docs = searcher.Search(query, hitsPerPage);
            ScoreDoc[] hits = docs.ScoreDocs;

            Console.WriteLine("Found {0} hits.", hits.Length);
            for (int i = 0; i < hits.Length; i++)
            {
                int docId = hits[i].Doc;
                Document d = searcher.Doc(docId);
                Console.WriteLine("{0}. {1}\t{2}", i + 1, d.Get("isbn"), d.Get("title"));

            }

            reader.Dispose();
        }

        static void addDoc(IndexWriter w, string title, string isbn)
        {
            Document doc = new Document();
            doc.Add(new TextField("title", title, Field.Store.YES));
            doc.Add(new StringField("isbn", isbn, Field.Store.YES));
            w.AddDocument(doc);
        }
    }
}
