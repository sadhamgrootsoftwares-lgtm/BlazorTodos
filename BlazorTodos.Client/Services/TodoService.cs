using System.Net.Http.Json;
using BlazorTodos.Shared.DTOs;

namespace BlazorTodos.Client.Services
{
    public class TodoService
    {
        private readonly HttpClient _httpClient;

        public TodoService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<TodoResponseDto>?> GetTodos()
        {
            return await _httpClient.GetFromJsonAsync<List<TodoResponseDto>>("api/todo");
        }

        public async Task<bool> CreateTodo(CreateTodoDto dto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/todo", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task DeleteTodo(int id)
        {
            await _httpClient.DeleteAsync($"api/todo/{id}");
        }

        public async Task ToggleTodo(int id)
        {
            await _httpClient.PutAsync($"api/todo/{id}/toggle", null);
        }
        public async Task<bool> UpdateTodo(int id, CreateTodoDto dto)
        {
            var response = await _httpClient.PutAsJsonAsync($"api/todo/{id}", dto);
            return response.IsSuccessStatusCode;
        }
    }
}