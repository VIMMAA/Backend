using Api.Context;
using Api.Models;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers;

[Route("api/[controller]")]
public class ScheduleController : ControllerBase
{
    private readonly ApplicationContext _context;

    public ScheduleController(ApplicationContext context)
    {
        _context = context;
    }

    private async Task CreateScheduleAsync()
    {
        var lessonNames = new List<string>
        {
            "Программирование",
            "Основы командной разработки",
            "Базы данных",
            "Разработка серверных приложений",
            "Физкультура",
            "Машинное обучение",
            "1С разработка",
            "Иностранный язык",
            "Web-разработка приложений",
            "Тестирование программного обеспечения"
        };

        var lessonStartAndEnd = new List<(TimeOnly, TimeOnly)>
        {
            (new TimeOnly(8, 45), new TimeOnly(10, 20)),
            (new TimeOnly(10, 35), new TimeOnly(12, 10)),
            (new TimeOnly(12, 25), new TimeOnly(14, 0)),
            (new TimeOnly(14, 45), new TimeOnly(16, 20)),
            (new TimeOnly(16, 35), new TimeOnly(18, 10)),
            (new TimeOnly(18, 25), new TimeOnly(20, 0)),
            (new TimeOnly(20, 15), new TimeOnly(21, 50))
        };

        var startDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddYears(1);
        var random = new Random();

        for (var date = startDate; date < endDate; date = date.AddDays(1))
        {
            var lessonCount = random.Next(2, 8);

            for (var i = 0; i < lessonCount; i++)
            {
                var lessonStart = lessonStartAndEnd[i].Item1;
                var lessonEnd = lessonStartAndEnd[i].Item2;

                var lesson = new Lesson
                {
                    Id = Guid.NewGuid(),
                    Name = lessonNames[random.Next(lessonNames.Count)],
                    StartTime = date.Add(lessonStart.ToTimeSpan()),
                    EndTime = date.Add(lessonEnd.ToTimeSpan())
                };

                _context.Lessons.Add(lesson);
            }
        }

        await _context.SaveChangesAsync();
    }

    [HttpGet]
    public async Task<IActionResult> GetScheduleAsync(GetScheduleModel model)
    {
        if (!await _context.Lessons.AnyAsync())
        {
            await CreateScheduleAsync();
        }

        var schedule = await _context.Lessons
            .Where(lesson => lesson.StartTime >= model.DateFrom && lesson.EndTime <= model.DateTo)
            .OrderBy(lesson => lesson.StartTime)
            .ToListAsync();

        return Ok(schedule);
    }
}