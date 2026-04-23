import { useState } from 'react';
import axios from 'axios';
import type { CatFactResponse } from '../types/api';
import { formatAxiosError } from '../utils/apiErrors';

// 1. Описываем, что возвращает хук (удобно для IDE и переиспользования)
export type UseCatFactReturn = {
  fact: string;
  error: string;
  loading: boolean;
  fetchFact: () => Promise<void>;
};

// 2. Сам хук. Имя ДОЛЖНО начинаться с 'use'
export const useCatFact = (): UseCatFactReturn => {
  // Состояния теперь живут внутри хука
  const [fact, setFact] = useState<string>('');
  const [error, setError] = useState<string>('');
  const [loading, setLoading] = useState<boolean>(false);

  // Логика запроса полностью переехала сюда
  const fetchFact = async () => {
    setLoading(true);
    setError('');
    setFact('');

    try {
      const response = await axios.get<CatFactResponse>('https://catfact.ninja/fact');
      setFact(response.data.fact);
    } catch (err) {
      setError(formatAxiosError(err));
    } finally {
      setLoading(false);
    }
  };

  // 3. Возвращаем данные и метод обновления
  return { fact, error, loading, fetchFact };
};