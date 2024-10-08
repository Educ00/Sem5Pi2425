using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sem5Pi2425.Domain.Shared;

namespace Sem5Pi2425.Domain.SystemUser {
    public class UserService {
        
        // ACRESCENTEM METODOS QUE PRECISEM DE LOGICA E DE MEXER COM OBJETOS DO DOMAIN
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserRepository _repo;

        public UserService(IUnitOfWork unitOfWork, IUserRepository repo) {
            this._unitOfWork = unitOfWork;
            this._repo = repo;
        }

        public async Task<List<UserDto>> GetAllAsync() {
            var list = await this._repo.GetAllAsync();
            
            List<UserDto> listDto = list.ConvertAll(user =>
                new UserDto(
                    user.Id,
                    user.Active,
                    user.Username,
                    user.Email,
                    user.FullName,
                    user.PhoneNumber,
                    user.Role));
            // Para testar se funfa só tirar o comentário. Deve aparecer.
            //listDTO.Add(new UserDto(new Guid(), true, new Username("aa"), new Email("teste@gmail.com"), new FullName("Joaquim Da Costa Queiroz"), new PhoneNumber("969999999"), Role.Admin));
            return listDto;
        }
        public async Task<ActionResult<UserDto>> GetByIdAssync(UserId id) {
            var user = await this._repo.GetByIdAsync(id);

            return user == null ? null : new UserDto(user);
        }

        public async Task<ActionResult<UserDto>> AddAsync(UserDto dto) {
            var userId = UserId.NewUserId();
            var user = new User(userId, dto.Username, dto.Email, dto.FullName, dto.PhoneNumber, dto.Role);
            // Para testar se funfa só tirar o comentário. Deve aparecer.
            //user = new User(userId, new Username("aa"), new Email("teste@gmail.com"), new FullName("Joaquim Da Costa Queiroz"), new PhoneNumber("969999999"), Role.Admin);
            await this._repo.AddAsync(user);
            await this._unitOfWork.CommitAsync();
            return new UserDto(user.Id, user.Active, user.Username, user.Email, user.FullName,
                user.PhoneNumber, user.Role);
        }

        public async Task<UserDto> InactivateAsync(UserId userId) {
            var user = await this._repo.GetByIdAsync(userId);

            if (user == null) {
                return null;
            }
            
            user.MarkAsInative();
            await this._unitOfWork.CommitAsync();
            return new UserDto(user.Id, user.Active, user.Username, user.Email, user.FullName, user.PhoneNumber,
                user.Role);
        }

        public async Task<UserDto> ActivateAsync(UserId userId) {
            var user = await this._repo.GetByIdAsync(userId);

            if (user == null) {
                return null;
            }
            
            user.MarkAsInative();
            await this._unitOfWork.CommitAsync();
            return new UserDto(user.Id, user.Active, user.Username, user.Email, user.FullName, user.PhoneNumber,
                user.Role);
        }
        
        public async Task<UserDto> DeleteAsync(UserId userId) {
            var user = await this._repo.GetByIdAsync(userId);

            if (user == null) {
                return null;
            }

            if (user.Active) {
                throw new BusinessRuleValidationException("It is not possible to delete an active user.");
            }
            
            this._repo.Remove(user);
            await this._unitOfWork.CommitAsync();

            return new UserDto(user.Id, user.Active, user.Username, user.Email, user.FullName, user.PhoneNumber,
                user.Role);
        }
    }
}