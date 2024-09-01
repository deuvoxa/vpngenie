using Humanizer;
using vpngenie.Domain.Entities;
using vpngenie.Domain.Interfaces;

namespace vpngenie.Application.Services;

public class PromocodeService(IPromocodeRepository promocodeRepository, UserService userService)
{
    public async Task<string> ActivatePromocode(string code, long telegramId)
    {
        code = code.Trim();
        var promocodes = await promocodeRepository.GetAll();
        var promocode = promocodes.FirstOrDefault(p => p.Code == code);
        
        if (promocode is null) return "Неизвестный промокод.";
        if (promocode.PromocodeUsages.Any(u => u.TelegramId == telegramId))
            return "Промокод уже использован.";
        if (promocode.Usages <= 0) return "Промокод закончился.";
        if (promocode.ValidTo < DateTime.UtcNow) return "Время действие промокода подошло к концу.";

        var user = await userService.GetUserByTelegramIdAsync(telegramId);
        
        promocode.PromocodeUsages.Add(user);
        promocode.Usages -= 1;
        await promocodeRepository.Update(promocode);

        await userService.ExtendSubscriptionAsync(user, promocode.BonusAmount);

        return $"Промокод успешно использован. Добавлено {TimeSpan.FromDays(promocode.BonusAmount).Humanize()}.\n" +
               $"Ваша подписка закончится {user.SubscriptionEndDate.Humanize()}";
    }

    public async Task AddPromocode(string code, int bouns, int usages, int validTo)
    {
        var promocode = new Promocode()
        {
            Id = Guid.NewGuid(),
            Code = code,
            BonusAmount = bouns,
            Usages = usages,
            ValidTo = DateTime.UtcNow.AddDays(validTo)
        };

        await promocodeRepository.Create(promocode);
    }
}