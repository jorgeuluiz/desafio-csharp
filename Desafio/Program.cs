using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Desafio
{
    class Program
    {

        public static void Main(string[] args)
        {

            var requisicaoWeb = WebRequest.CreateHttp("https://api.codenation.dev/v1/challenge/dev-ps/generate-data?token=f49e701e216e7efc113c101788a4825b7a3afd97");
            requisicaoWeb.Method = "GET";

            using (var resposta = requisicaoWeb.GetResponse())
            {
                var streamDados = resposta.GetResponseStream();
                StreamReader reader = new StreamReader(streamDados);
                object objResponse = reader.ReadToEnd();
                Console.WriteLine(objResponse.ToString());

                var post = JsonConvert.DeserializeObject<Post>(objResponse.ToString());

                string textoCifrado = post.cifrado;
                List<string> arrayAlfabeto = new List<string> { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
                StringBuilder textoFinal = new StringBuilder(textoCifrado);
                for (int i = 0; i < textoFinal.Length; i++)
                {
                    var aux = textoFinal[i].ToString();

                    if (arrayAlfabeto.IndexOf(aux) > -1)
                    {
                        int posicaoLetra = arrayAlfabeto.IndexOf(aux) + 3;
                        if (posicaoLetra <= -1)
                            posicaoLetra = arrayAlfabeto.Count + posicaoLetra;
                        textoFinal.Remove(i, 1);
                        textoFinal.Insert(i, arrayAlfabeto[posicaoLetra].ToString());
                    }
                }

                Console.WriteLine(textoCifrado);
                Console.WriteLine(textoFinal);

                Console.WriteLine(CalculateSHA1(textoFinal.ToString()).ToLower());
                RequisiscaoPost();
                Console.ReadLine();
                streamDados.Close();
                resposta.Close();

            }
            Console.ReadLine();

            
                        
        }

        public static string CalculateSHA1(string text)
        {
            try
            {
                byte[] buffer = Encoding.Default.GetBytes(text);
                System.Security.Cryptography.SHA1CryptoServiceProvider cryptoTransformSHA1 = new System.Security.Cryptography.SHA1CryptoServiceProvider();
                string hash = BitConverter.ToString(cryptoTransformSHA1.ComputeHash(buffer)).Replace("-", "");
                return hash;
            }
            catch (Exception x)
            {
                throw new Exception(x.Message);
            }
        }

        public static async void RequisiscaoPost()
        {
            HttpClient httpClient = new HttpClient();
            MultipartFormDataContent form = new MultipartFormDataContent();

            var file_bytes = File.ReadAllBytes(@"C:\Users\Junior\Desktop\desafio codenation\answer.json");

            form.Add(new ByteArrayContent(file_bytes, 0, file_bytes.Length), "answer", "answer.json");
            HttpResponseMessage response = await httpClient.PostAsync("https://api.codenation.dev/v1/challenge/dev-ps/submit-solution?token=f49e701e216e7efc113c101788a4825b7a3afd97", form);

            response.EnsureSuccessStatusCode();
            httpClient.Dispose();
            string sd = response.Content.ReadAsStringAsync().Result;
        }
    }
}

