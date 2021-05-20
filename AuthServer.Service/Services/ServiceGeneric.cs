using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using SharedLibrary.Dtos;

namespace AuthServer.Service.Services
{
    public class ServiceGeneric<TEntity, TDto>: IServiceGeneric<TEntity, TDto> where TEntity: class where TDto: class
    {
        /// <summary>
        /// Dependency injection are added in the core api startup.cs
        /// </summary>
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<TEntity> _genericRepository;
        
        public ServiceGeneric(IUnitOfWork unitOfWork, IGenericRepository<TEntity> genericRepository)
        {
            _unitOfWork = unitOfWork;
            _genericRepository = genericRepository;
        }

        public async Task<Response<TDto>> AddAsync(TDto entity)
        {
            var newEntity = ObjectMapper.Mapper.Map<TEntity>(entity);
            // add to database
            await _genericRepository.AddAsync(newEntity);
            // commit changes
            await _unitOfWork.CommitAsync();

            var newDto = ObjectMapper.Mapper.Map<TDto>(newEntity);
            
            return Response<TDto>.Success(newDto, 200);
        }
        
        public async Task<Response<IEnumerable<TDto>>> GetAllAsync()
        {
            var products = ObjectMapper.Mapper.Map<List<TDto>>(await _genericRepository.GetAllAsync());
            return Response<IEnumerable<TDto>>.Success(products, 200);
        }

        public async Task<Response<TDto>> GetByIdAsync(int id)
        {
            var product = await _genericRepository.GetByIdAsync(id);
           
            if (product == null)
            {
                return Response<TDto>.Fail($"Id: {id} not Found", 404, true);
            }

            var productDto = ObjectMapper.Mapper.Map<TDto>(product);

            return Response<TDto>.Success(productDto, 200);
        }

        public async Task<Response<NoDataDto>> Remove(int id)
        {
            var existingEntity = await _genericRepository.GetByIdAsync(id);

            if (existingEntity == null)
            {
                return Response<NoDataDto>.Fail($"Id: {id} not Found", 404, true);
            }
            _genericRepository.Remove(existingEntity);
            
            // apply changes
            await _unitOfWork.CommitAsync();
            
            return Response<NoDataDto>.Success(200);
        }
        
        public async Task<Response<NoDataDto>> Update(TDto entity, int id)
        {
            var existingEntity = await _genericRepository.GetByIdAsync(id);

            if (existingEntity == null)
            {
                return Response<NoDataDto>.Fail($"Id: {id} not Found", 404, true);
            }

            var updateEntity = ObjectMapper.Mapper.Map<TEntity>(entity);

            _genericRepository.Update(updateEntity);
            
            // commit changes
            await _unitOfWork.CommitAsync();
            return Response<NoDataDto>.Success(204);
        }
        
        public async Task<Response<IEnumerable<TDto>>> Where(Expression<Func<TEntity, bool>> predicate)
        {
            // where(x => x.id > 5)
            var list = _genericRepository.Where(predicate);
            // list.Skip(4).Take(5);
            return Response<IEnumerable<TDto>>.Success(ObjectMapper.Mapper.Map<IEnumerable<TDto>>(await list.ToListAsync()), 200);
        }
    }
}