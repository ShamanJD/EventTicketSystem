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
        throw new Error('Booking failed');
    }

    return response.json();
};
