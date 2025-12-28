import { useState } from 'react';
import { useMutation } from '@tanstack/react-query';
import { createBooking } from './api/bookingsApi';
import { useBookingUpdates } from './hooks/useBookingUpdates';

const BookingPage = () => {
    const [bookingId, setBookingId] = useState(null);
    // Hardcoded demo values
    const concertId = "3fa85f64-5717-4562-b3fc-2c963f66afa6";
    const amount = 100;

    const mutation = useMutation({
        mutationFn: () => createBooking(concertId, amount),
        onSuccess: (data) => {
            console.log("Booking created:", data);
            // Assuming data contains { id: "..." }
            setBookingId(data.id || data.bookingId);
        }
    });

    const status = useBookingUpdates(bookingId);

    const getStatusColor = (s) => {
        if (s === 'Confirmed') return 'bg-green-500';
        if (s === 'Cancelled') return 'bg-red-500';
        return 'bg-yellow-500';
    };

    return (
        <div className="p-8">
            <h2 className="text-2xl font-bold mb-4">Бронирование билета (Демо)</h2>

            <button
                onClick={() => mutation.mutate()}
                disabled={mutation.isPending || bookingId}
                className="px-4 py-2 bg-blue-600 text-white rounded hover:bg-blue-700 disabled:bg-gray-400"
            >
                {mutation.isPending ? 'Обработка...' : 'Забронировать'}
            </button>

            {mutation.isError && (
                <div className="mt-4 text-red-500">Ошибка: {mutation.error.message}</div>
            )}

            {bookingId && (
                <div className="mt-6 flex items-center space-x-4">
                    <span className="font-semibold">ID Брони: {bookingId}</span>
                </div>
            )}

            {(bookingId || status) && (
                <div className="mt-4">
                    <span className={`inline-block px-3 py-1 rounded-full text-white ${getStatusColor(status || 'Processing')}`}>
                        Статус: {status || 'Awaiting Payment...'}
                    </span>
                </div>
            )}
        </div>
    );
};

export default BookingPage;
