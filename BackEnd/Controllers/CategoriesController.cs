﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using MySqlConnector;
using BackEnd.Model;
namespace BackEnd.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        protected MySqlConnection? conn;
        [HttpGet]
        public IActionResult Get()
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    var sqlcommand = "SELECT * FROM Category";
                    var result = conn.Query<Category>(sqlcommand);
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
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    var sqlcommand = $"SELECT * FROM Category WHERE id = @Id";
                    var result = conn.QueryFirstOrDefault<Category>(sqlcommand, param: new { Id = id });
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
        public IActionResult Post(Category c)
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    var sqlcommand = $"INSERT INTO Category(name, description) VALUES(@name, @description)";
                    DynamicParameters dynamic = new DynamicParameters();
                    dynamic.Add("@name", c.name);
                    dynamic.Add("@description", c.description);
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
        public IActionResult Put(Category c, int id)
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    var sqlcommand = $"UPDATE Category SET name=@name,description=@description WHERE id=@id";
                    DynamicParameters dynamic = new DynamicParameters();
                    dynamic.Add("@id", id);
                    dynamic.Add("@name", c.name);
                    dynamic.Add("@description", c.description);
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
                    var sqlcommand1 = $"DELETE FROM Category WHERE id = @Id";
                    var result = conn.Execute(sqlcommand1, param: new { Id = id });
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
        [HttpGet("search/{s}")]
        public IActionResult Search(string s)
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    DynamicParameters para = new DynamicParameters();
                    para.Add("@search", s);
                    var sqlCommand = $"SELECT * FROM Category WHERE name LIKE CONCAT('%',@search,'%') ORDER BY id LIMIT 10";
                    var sqlCommand1 = $"SELECT * FROM Category WHERE name LIKE CONCAT('%',@search,'%')";
                    var total = conn.Query<Category>(sqlCommand1, param: para).Count();
                    var result = conn.Query<Category>(sqlCommand, param: para);
                    return StatusCode(
                        0, new
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
            
        }
        [HttpGet("pagination")]
        public IActionResult Pagination(int PageNumber, string? filter)
        {
            try
            {
                using (conn = new MySqlConnection(SqlConnectionString.ConnectionString))
                {
                    string sqlCommand = "";
                    int x = (PageNumber - 1) * 10;
                    int TotalRecord = 0;
                    DynamicParameters para = new DynamicParameters();
                    para.Add("@filter", filter);
                    para.Add("@PageNumber", x);
                    if (string.IsNullOrEmpty(filter))
                    {
                        sqlCommand = $"SELECT * FROM Category ORDER BY id LIMIT 10 OFFSET @PageNumber";
                        TotalRecord = conn.Query<Category>("Select * from Category").Count();
                    }
                    else
                    {
                        sqlCommand = $"SELECT * FROM Category WHERE name LIKE CONCAT('%',@filter,'%') ORDER BY id LIMIT 10 OFFSET @PageNumber";
                        TotalRecord = conn.Query<Category>("Select * from Category WHERE name LIKE CONCAT('%',@filter,'%')", param: para).Count();
                    }
                    float tp = (float)TotalRecord / 10;
                    int TotalPage = (int)Math.Ceiling(tp);
                    var result = conn.Query<Category>(sqlCommand, param: para);
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
