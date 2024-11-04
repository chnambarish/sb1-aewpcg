using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MSS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DataController : ControllerBase
{
    private static readonly List<DataItem> _items = new();

    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_items);
    }

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var item = _items.FirstOrDefault(x => x.Id == id);
        return item == null ? NotFound() : Ok(item);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Create([FromBody] DataItem item)
    {
        item.Id = _items.Count + 1;
        _items.Add(item);
        return CreatedAtAction(nameof(Get), new { id = item.Id }, item);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public IActionResult Update(int id, [FromBody] DataItem item)
    {
        var existingItem = _items.FirstOrDefault(x => x.Id == id);
        if (existingItem == null) return NotFound();
        
        existingItem.Name = item.Name;
        existingItem.Description = item.Description;
        return Ok(existingItem);
    }
}