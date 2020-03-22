using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Library.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Xunit;

namespace XUnitTestProject1.Controllers
{
    public class BookBorrowsUnitTest
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;


        public BookBorrowsUnitTest()
        {
            _server = ServerFactory.GetServerInstance();
            _client = _server.CreateClient();


            using (var scope = _server.Host.Services.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<LibraryContext>();
                _db.Author.Add(new Author
                {
                    IdAuthor = 1,
                    Name = "Test",
                    Surname = "Testowy"
                });

                _db.Book.Add(new Book
                {
                    IdBook =1,
                    Title = "Test Book",
                    IdAuthor = 1,
                    PublishYear = "1997"
                });
                _db.BookBorrow.Add(new BookBorrow
                {
                    IdUser = 1,
                    IdBook = 1,
                    BorrowDate = DateTime.Today,
                    ReturnDate = DateTime.Today,
                    Comments = "No Comments"
                });
              

                _db.SaveChanges();
            }
        }


        [Fact]
        public async Task PostBookBorrows_200Ok()
        {
            

            var cc = _client;
            string bookBorrowObject = JsonConvert.SerializeObject(new
            {
                IdUser = 1,
                IdBook = 1,
                BorrowDate = DateTime.Today,
                ReturnDate = DateTime.Today,
                Comments = "No Coomments"
            });
            var jsonObject = new StringContent(bookBorrowObject, Encoding.UTF8, "application/json");
            var httpResponse = await cc.PostAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows", jsonObject);

            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            Assert.True(httpResponse.StatusCode == HttpStatusCode.Created);
            
        }
        [Fact]
        public async Task PutBookBorrows_200Ok()
        {
            var cc = _client;
            var id = 1;
            string bookBorrowObject = JsonConvert.SerializeObject(new
            {
                IdUser = 1,
                IdBook = 1,
                BorrowDate = DateTime.Today,
                ReturnDate = DateTime.Today,
                Comments = "More Comments"
            });
            var jsonObject = new StringContent(bookBorrowObject, Encoding.UTF8, "application/json");
            var httpResponse = await cc.PutAsync($"{_client.BaseAddress.AbsoluteUri}api/book-borrows/{id}", jsonObject);
            httpResponse.EnsureSuccessStatusCode();
            var content = await httpResponse.Content.ReadAsStringAsync();
            Assert.True(httpResponse.StatusCode == HttpStatusCode.NoContent);
        }

        


    }
}
