using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace UserApp.Controllers
{
    public class Network : NetModelsLibrary.Network
    {
        List<string> stack = new List<string>();

        TaskCompletionSource<string> tcs;

        public Network(NetworkStream stream) : base(stream)
        {
            tcs = new TaskCompletionSource<string>();
            Task.Factory.StartNew(Process);
        }

        public readonly Regex TypeRegEx = new Regex(@"<\/([^>]+)>$");

        public void Stop()
        {
            tcs.SetException(new Exception());
        }
        private void Process()
        {
            try
            {
                while (true)
                {
                    var res = ReadRaw(); //прочитати рядок
                    stack.Add(res);//додати в стек на випадок якщо поточни результат не отримають одразу
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public override T ReadObject<T>()
        {
            try
            {
                var Tname = typeof(T).Name; // отримати назву того типу, який нам саме і потрібен для подальшого порівняння
                do
                {
                    string? res = null;
                    for (int i = 0; i < stack.Count; i++)
                    {
                        //перевірка співпадіння записів у стеку
                        if (stack[i]!=null)
                        {
                            if (Tname == TypeRegEx.Match(stack[i]).Groups[1].Value)
                            {
                                //якщо знайшли видалити зі стеку та повернути
                                res = stack[i];
                                stack.Remove(stack[i]);
                                return base.ReadObject<T>(res);
                            }
                        }
                        
                    }
                } while (true);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    ex.Message + ex.StackTrace,
                    "Exeption", System.Windows.MessageBoxButton.OK,
                    System.Windows.MessageBoxImage.Error);
                throw;
            }
            
        }
    }
}
