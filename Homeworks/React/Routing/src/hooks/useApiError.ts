// src/hooks/useApiError.ts
import { useState, useCallback } from 'react';
import { formatAxiosError } from '../utils/apiErrors';

export const useApiError = () => {
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState(false);

  // Функция-обёртка для безопасного выполнения запросов
  const handleRequest = useCallback(async <T>(promise: Promise<T>): Promise<T | null> => {
    setIsLoading(true);
    setError(null);
    
    try {
      const result = await promise;
      return result;
    } catch (err) {
      setError(formatAxiosError(err));
      return null; // Компонент сам решит, как реагировать на null
    } finally {
      setIsLoading(false);
    }
  }, []);

  return { error, isLoading, setError, handleRequest };
};