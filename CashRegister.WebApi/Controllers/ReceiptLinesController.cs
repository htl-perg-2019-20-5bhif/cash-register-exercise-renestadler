using CashRegister.Shared;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CashRegister.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptLinesController : ControllerBase
    {
        private readonly ProductDataContext _context;

        public ReceiptLinesController(ProductDataContext context)
        {
            _context = context;
        }

        // GET: api/ReceiptLines
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReceiptLine>>> GetReceiptLines()
        {
            return await _context.ReceiptLines.ToListAsync();
        }

        // GET: api/ReceiptLines/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ReceiptLine>> GetReceiptLine(int id)
        {
            var receiptLine = await _context.ReceiptLines.FindAsync(id);

            if (receiptLine == null)
            {
                return NotFound();
            }

            return receiptLine;
        }

        // PUT: api/ReceiptLines/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReceiptLine(int id, ReceiptLine receiptLine)
        {
            if (id != receiptLine.ReceiptLineId)
            {
                return BadRequest();
            }

            _context.Entry(receiptLine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ReceiptLineExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/ReceiptLines
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost]
        public async Task<ActionResult<ReceiptLine>> PostReceiptLine(ReceiptLine receiptLine)
        {
            _context.ReceiptLines.Add(receiptLine);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetReceiptLine", new { id = receiptLine.ReceiptLineId }, receiptLine);
        }

        // POST: api/Receipts/All
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("All")]
        public async Task<ActionResult<Receipt>> PostAllReceipts(List<ReceiptLine> receiptLines)
        {
            receiptLines.ForEach(r => r.ProductId = r.Product.ProductId);
            receiptLines.ForEach(r => r.Product = null);
            _context.ReceiptLines.AddRange(receiptLines);

            var receipt = new Receipt { ReceiptLines = receiptLines, ReceiptTimestamp = DateTime.Now.Ticks, TotalPrice = receiptLines.Sum(r => r.TotalPrice) };
            _context.Receipts.Add(receipt);
            await _context.SaveChangesAsync();

            return receipt;
        }

        // DELETE: api/ReceiptLines/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ReceiptLine>> DeleteReceiptLine(int id)
        {
            var receiptLine = await _context.ReceiptLines.FindAsync(id);
            if (receiptLine == null)
            {
                return NotFound();
            }

            _context.ReceiptLines.Remove(receiptLine);
            await _context.SaveChangesAsync();

            return receiptLine;
        }

        private bool ReceiptLineExists(int id)
        {
            return _context.ReceiptLines.Any(e => e.ReceiptLineId == id);
        }
    }
}
