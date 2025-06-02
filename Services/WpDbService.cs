using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Eridian_Websites.Services
{
    public class WpDbService
    {
        private readonly string _connectionString;

        public WpDbService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("WordPressDb");
        }

        public async Task<List<string>> GetProductTitlesAsync()
        {
            var productTitles = new List<string>();

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            string query = "SELECT post_title FROM wp_posts WHERE post_type = 'product' AND post_status = 'publish';";

            using var command = new MySqlCommand(query, connection);
            using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                productTitles.Add(reader.GetString(0));
            }

            return productTitles;
        }

        public async Task<long> InsertProductAsync(string title, string description, decimal price, int stock, string imageUrl)
        {
            long postId;

            using var connection = new MySqlConnection(_connectionString);
            await connection.OpenAsync();

            using var transaction = await connection.BeginTransactionAsync();

            try
            {
                // 1. Insert product post
                var insertPost = @"
                    INSERT INTO wp_posts (
                        post_author, post_date, post_date_gmt, post_content, post_title, 
                        post_status, comment_status, ping_status, post_name, post_type
                    ) VALUES (
                        1, NOW(), NOW(), @description, @title,
                        'publish', 'closed', 'closed', @slug, 'product'
                    );
                    SELECT LAST_INSERT_ID();
                ";

                using var cmd = new MySqlCommand(insertPost, connection, (MySqlTransaction)transaction);
                cmd.Parameters.AddWithValue("@title", title);
                cmd.Parameters.AddWithValue("@description", description);
                cmd.Parameters.AddWithValue("@slug", title.ToLower().Replace(" ", "-"));

                postId = Convert.ToInt64(await cmd.ExecuteScalarAsync());

                // 2. Insert image as attachment
                long imagePostId = 0;
                if (!string.IsNullOrWhiteSpace(imageUrl))
                {
                    var imageName = Path.GetFileName(imageUrl);

                    var insertImage = @"
                        INSERT INTO wp_posts (
                            post_author, post_date, post_date_gmt, post_title, post_status, 
                            post_name, post_parent, guid, post_type, post_mime_type
                        ) VALUES (
                            1, NOW(), NOW(), @imageName, 'inherit',
                            @imageSlug, @parentId, @guid, 'attachment', 'image/jpeg'
                        );
                        SELECT LAST_INSERT_ID();
                    ";

                    using var imgCmd = new MySqlCommand(insertImage, connection, (MySqlTransaction)transaction);
                    imgCmd.Parameters.AddWithValue("@imageName", imageName);
                    imgCmd.Parameters.AddWithValue("@imageSlug", Path.GetFileNameWithoutExtension(imageName).ToLower().Replace(" ", "-"));
                    imgCmd.Parameters.AddWithValue("@parentId", postId);
                    imgCmd.Parameters.AddWithValue("@guid", imageUrl);

                    imagePostId = Convert.ToInt64(await imgCmd.ExecuteScalarAsync());

                    // Set _thumbnail_id for product
                    var thumbCmd = new MySqlCommand(@"
                INSERT INTO wp_postmeta (post_id, meta_key, meta_value)
                VALUES (@productId, '_thumbnail_id', @imagePostId);
            ", connection, (MySqlTransaction)transaction);

                    thumbCmd.Parameters.AddWithValue("@productId", postId);
                    thumbCmd.Parameters.AddWithValue("@imagePostId", imagePostId);
                    await thumbCmd.ExecuteNonQueryAsync();
                }

                // 3. Insert product meta
                var metaPairs = new Dictionary<string, string>
                {
                    { "_price", price.ToString() },
                    { "_regular_price", price.ToString() },
                    { "_stock", stock.ToString() },
                    { "_stock_status", stock > 0 ? "instock" : "outofstock" },
                    { "_manage_stock", "yes" },
                };

                foreach (var pair in metaPairs)
                {
                    var metaCmd = new MySqlCommand(
                        "INSERT INTO wp_postmeta (post_id, meta_key, meta_value) VALUES (@postId, @key, @value);",
                        connection, (MySqlTransaction)transaction);

                    metaCmd.Parameters.AddWithValue("@postId", postId);
                    metaCmd.Parameters.AddWithValue("@key", pair.Key);
                    metaCmd.Parameters.AddWithValue("@value", pair.Value);

                    await metaCmd.ExecuteNonQueryAsync();
                }

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }

            return postId;
        }



    }
}
