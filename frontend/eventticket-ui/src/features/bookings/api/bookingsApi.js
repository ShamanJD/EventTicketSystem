import { authorizedFetch } from '../../../api';

export const createBooking = async (concertId, amount) => {
    const response = await authorizedFetch('/bookings', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ concertId, amount }),
    });

    if (!response.ok) {
        const errorData = await response.json().catch(() => ({}));
        const errorMessage = errorData.error || errorData.Error || errorData.message || 'Ошибка при создании бронирования';
        throw new Error(errorMessage);
    }

    return response.json();
};
