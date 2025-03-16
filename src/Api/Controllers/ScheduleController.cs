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

    private bool _isScheduleCreated = false;

    public ScheduleController(ApplicationContext context)
    {
        _context = context;
    }

    private async Task CreateScheduleAsync() // TODO: Парсинг с расписания
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

        var startDate = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = startDate.AddYears(1);
        var random = new Random();

        for (var date = startDate; date < endDate; date = date.AddDays(1))
        {
            var lessonCount = random.Next(2, 8);

            for (var i = 0; i < lessonCount; i++)
            {
                var lessonStart = date.Date.AddHours(8.75).AddHours(1.5 * i);
                var lessonEnd = lessonStart.AddHours(1.5);

                var lesson = new Lesson
                {
                    Id = Guid.NewGuid(),
                    Name = lessonNames[random.Next(lessonNames.Count)],
                    StartTime = lessonStart,
                    EndTime = lessonEnd
                };

                _context.Lessons.Add(lesson);
            }
        }

        await _context.SaveChangesAsync();
    }

    [HttpPost]
    public async Task<IActionResult> GetScheduleAsync([FromBody] GetScheduleModel model)
    {
        if (!_isScheduleCreated)
        {
            await CreateScheduleAsync();
            _isScheduleCreated = true;
        }

        var schedule = await _context.Lessons
            .Where(lesson => lesson.StartTime >= model.DateFrom && lesson.EndTime <= model.DateTo)
            .OrderBy(lesson => lesson.StartTime)
            .ToListAsync();

        return Ok(schedule);
    }
}