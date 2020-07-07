using System;

namespace Abc.HabitTracker
{
  public interface UnitOfWork : IDisposable
  {
    void Commit();
    void Rollback();
  }
}