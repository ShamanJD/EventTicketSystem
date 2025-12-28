import { useQuery } from '@tanstack/react-query';
import { getConcerts } from '../api/concertsApi';

export const useConcerts = () => {
    return useQuery({
        queryKey: ['concerts'],
        queryFn: getConcerts,
        staleTime: 5 * 60 * 1000, // 5 minutes
    });
};
