using FluentValidation;
using TaskFlow.Application.DTOs.Requests;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Validators;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskRequest>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .MaximumLength(500);

        RuleFor(x => x.Priority)
            .Must(x => Enum.IsDefined(typeof(AppTaskPriority), x))
            .WithMessage("Priority must be a valid value (0=Low, 1=Medium, 2=High, 3=Critical)");

        RuleFor(x => x.DueDate)
            .GreaterThan(DateTime.UtcNow)
            .WithMessage("Due date must be in the future")
            .When(x => x.DueDate.HasValue);

        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .When(x => x.Description is not null);

        RuleFor(x => x.Status)
            .Must(x => Enum.IsDefined(typeof(AppTaskStatus), x!))
            .WithMessage("Status must be a valid value (0=ToDo, 1=InProgress, 2=InReview, 3=Done)")
            .When(x => x.Status.HasValue);
    }
}