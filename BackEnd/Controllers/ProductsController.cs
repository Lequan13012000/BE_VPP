
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using BackEnd.Model;
using System.Collections;
namespace BackEnd.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        protected MySqlConnection? conn;
        [HttpGet]
        public IActionResult GetByCategoryId()
        {

            try
            {

                ArrayList result = new ArrayList();
                SortedList objectCate = new SortedList();
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    var sqlCategory = "SELECT * FROM Category";
                    var totalCategory = conn.Query<Category>(sqlCategory).Count();
                    var category = conn.Query<Category>(sqlCategory).ToList();

                    for (int i = 1; i < totalCategory; i++)
                    {
                        var sqlcommand = $"SELECT * FROM Product where cat_id = {i}";
                        var products = conn.Query<Product>(sqlcommand);
                        objectCate = new SortedList();
                        objectCate.Add($"list", products);
                        objectCate.Add($"name", category[i].name);
                        objectCate.Add($"image", category[i].image);
                        result.Add(objectCate);
                    }
                    return StatusCode(200, result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Có lỗi xảy ra vui lòng kiểm tra lại",
                    error = ex.Message
                });
            }
        }

        [HttpGet("Category/{catId}")]
        public IActionResult GetCat(int catId, int PageNumber, int limit)
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    int x = (PageNumber - 1) * 6;
                    int TotalRecord = 0;
                    var sqlcommand = "";
                    if(limit == 0)
                    {
                        sqlcommand = "SELECT * FROM Product WHERE cat_id = @Id ORDER BY id";
                    }
                    else
                    {
                    sqlcommand = $"SELECT * FROM Product WHERE cat_id = @Id ORDER BY id LIMIT {limit} OFFSET @PageNumber";
                    }
 
                    TotalRecord = conn.Query<Product>("Select * from Product WHERE cat_id = @Id", param: new { Id = catId }).Count();
                    var result = conn.Query<Product>(sqlcommand, param: new { Id = catId , PageNumber = x});
                    float tp = (float)TotalRecord / 6;
                    int TotalPage = (int)Math.Ceiling(tp);
                    return Ok(new
                    {
                        totalPage = TotalPage,
                        totalRecord = TotalRecord,
                        data = result,
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Có lỗi xảy ra vui lòng kiểm tra lại",
                    error = ex.Message
                });
            }
        }
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    var sqlcommand = $"SELECT * FROM Product WHERE id = @Id";
                    var result = conn.QueryFirstOrDefault<Product>(sqlcommand, param: new { Id = id });
                    return StatusCode(200, result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Có lỗi xảy ra vui lòng kiểm tra lại",
                    error = ex.Message
                });
            }

        }
        [HttpPost]
        public IActionResult Post(Product p)
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    var sqlcommand = $"INSERT INTO Product(cat_id, name, origin, price, image, description, amount, unit) VALUES(@cat_id, @name, @origin, @price, @image, @description, @amount, @unit)";
                    DynamicParameters dynamic = new DynamicParameters();
                    dynamic.Add("@cat_id", p.cat_id);
                    dynamic.Add("@name", p.name);
                    dynamic.Add("@origin", p.origin);
                    dynamic.Add("@price", p.price);
                    dynamic.Add("@image", p.image);
                    dynamic.Add("@description", p.description);
                    dynamic.Add("@amount", p.amount);
                    dynamic.Add("@unit", p.unit);
                    var result = conn.Execute(sqlcommand, param: dynamic);
                    return StatusCode(201, result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Có lỗi xảy ra vui lòng kiểm tra lại",
                    error = ex.Message
                });
            }

        }
        [HttpPut("{id}")]
        public IActionResult Put(Product p, int id)
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    var sqlcommand = $"UPDATE Product SET cat_id=@cat_id,name=@name, origin=@origin,price=@price,image=@image,description=@description,amount=@amount,unit=@unit WHERE id=@id";
                    DynamicParameters dynamic = new DynamicParameters();
                    dynamic.Add("@id", id);
                    dynamic.Add("@cat_id", p.cat_id);
                    dynamic.Add("@name", p.name);
                    dynamic.Add("@origin", p.origin);
                    dynamic.Add("@price", p.price);
                    dynamic.Add("@image", p.image);
                    dynamic.Add("@description", p.description);
                    dynamic.Add("@amount", p.amount);
                    dynamic.Add("@unit", p.unit);
                    var result = conn.Execute(sqlcommand, param: dynamic);
                    return StatusCode(200, result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Có lỗi xảy ra vui lòng kiểm tra lại",
                    error = ex.Message
                });
            }

        }
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    var sqlcommand = $"DELETE FROM Product WHERE id = @Id";
                    var result = conn.Execute(sqlcommand, param: new { Id = id });
                    return StatusCode(200, result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Có lỗi xảy ra vui lòng kiểm tra lại",
                    error = ex.Message
                });
            }

        }
      /*  [HttpGet("search")]
        public IActionResult Search(string s)
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    DynamicParameters para = new DynamicParameters();
                    para.Add("@search", s);
                    var sqlCommand = $"SELECT * FROM Product WHERE name LIKE CONCAT('%',@search,'%') ORDER BY id LIMIT 10";
                    var sqlCommand1 = $"SELECT * FROM Product WHERE name LIKE CONCAT('%',@search,'%')";
                    var total = conn.Query<Product>(sqlCommand1, param: para).Count();
                    var result = conn.Query<Product>(sqlCommand, param: para);
                    return StatusCode(200, new
                    {
                        total = total,
                        data = result
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Có lỗi xảy ra vui lòng kiểm tra lại",
                    error = ex.Message
                });
            }

        }*/
        [HttpGet("search")]
        public IActionResult Pagination(int PageNumber, string? search_key, int limit)
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    string sqlCommand = "";
                    int x = (PageNumber - 1) * limit;
                    int TotalRecord = 0;

                    DynamicParameters para = new DynamicParameters();
   
                    para.Add("@filter", search_key);
                    para.Add("@PageNumber", x);
                    if (string.IsNullOrEmpty(search_key))
                    {            
                            sqlCommand = $"SELECT * FROM Product ORDER BY id DESC LIMIT {limit} OFFSET @PageNumber";
                            TotalRecord = conn.Query<Product>("Select * from Product", param: para).Count();
                    }
                    else
                    {
                            sqlCommand = $"SELECT * FROM Product WHERE name LIKE CONCAT('%',@filter,'%') ORDER BY id DESC LIMIT {limit} OFFSET @PageNumber";
                            TotalRecord = conn.Query<Product>("Select * from Product WHERE name LIKE CONCAT('%',@filter,'%')", param: para).Count();
                    }
                    float tp = (float)TotalRecord / limit;
                    int TotalPage = (int)Math.Ceiling(tp);
                    var result = conn.Query<Product>(sqlCommand, param: para);
                    return Ok(new
                    {
                        totalPage = TotalPage,
                        totalRecord = TotalRecord,
                        data = result,
                    });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Có lỗi xảy ra vui lòng kiểm tra lại",
                    error = ex.Message
                });
            }

        }
    }
}
