using Exadel.ReportHub.RA;
using Exadel.ReportHub.RA.Abstract;
using Exadel.ReportHub.SDK.DTOs.Plan;
using FluentValidation;

namespace Exadel.ReportHub.Handlers.Validators;
public class CreatePlanValidator : AbstractValidator<CreatePlanDTO>
{
    private readonly IValidator<UpdatePlanDTO> _updatePlanValidator;
    private readonly IItemRepository _itemRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IPlanRepository _planRepository;

    public CreatePlanValidator(
        IValidator<UpdatePlanDTO> updatePlanValidator,
        IItemRepository itemRepository,
        IClientRepository clientRepository,
        IPlanRepository planRepository)
    {
        _updatePlanValidator = updatePlanValidator;
        _itemRepository = itemRepository;
        _clientRepository = clientRepository;
        _planRepository = planRepository;
        ConfigureRules();
    }

    private void ConfigureRules()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;
        ClassLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x)
            .SetValidator(_updatePlanValidator)
            .MustAsync(IsUniquePlanAsync)
            .WithMessage(Constants.Validation.Plan.AlreadyExistsForItemAndClient);

        RuleFor(x => x.ItemId)
            .NotEmpty()
            .MustAsync(_itemRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Item.DoesNotExist);

        RuleFor(x => x.ClientId)
            .NotEmpty()
            .MustAsync(_clientRepository.ExistsAsync)
            .WithMessage(Constants.Validation.Client.DoesNotExist);
    }

    private async Task<bool> IsUniquePlanAsync(CreatePlanDTO createPlanDto, CancellationToken cancellationToken)
    {
        var isExists = await _planRepository.ExistsForItemByPeriodAsync(
            createPlanDto.ItemId, createPlanDto.ClientId, createPlanDto.StartDate, createPlanDto.EndDate, cancellationToken);
        return !isExists;
    }
}
