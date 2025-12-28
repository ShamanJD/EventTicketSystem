import { authorizedFetch } from '../../../api';

export const getConcerts = async () => {
    const response = await authorizedFetch('/concerts', {
        method: 'GET',
    });

    if (!response.ok) {
        throw new Error('Failed to fetch concerts');
    }

    return response.json();
};
