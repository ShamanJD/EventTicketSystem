const API_URL = 'https://localhost:7172/api';

export const authorizedFetch = async (url, options = {}) => {
    const accessToken = localStorage.getItem('accessToken');

    if (!accessToken || accessToken === "null" || accessToken === "undefined") {
        console.error("No valid token found");
        return new Response(JSON.stringify({ error: "Unauthorized" }), { status: 401 });
    }

    options.headers = {
        ...options.headers,
        'Authorization': `Bearer ${accessToken}`,
        'Content-Type': 'application/json'
    };

    try {
        let response = await fetch(`${API_URL}${url}`, options);

        if (response.status === 401) {
            const refreshToken = localStorage.getItem('refreshToken');

            if (!refreshToken || refreshToken === "null") {
                return response;
            }

            const refreshResponse = await fetch(`${API_URL}/auth/refresh`, {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({
                    accessToken: accessToken,
                    refreshToken: refreshToken
                })
            });

            if (refreshResponse.ok) {
                const data = await refreshResponse.json();
                localStorage.setItem('accessToken', data.accessToken);
                localStorage.setItem('refreshToken', data.refreshToken);

                options.headers['Authorization'] = `Bearer ${data.accessToken}`;
                return await fetch(`${API_URL}${url}`, options);
            } else {
                localStorage.removeItem('accessToken');
                localStorage.removeItem('refreshToken');
            }
        }
        return response;
    } catch (err) {
        console.error("Network error:", err);
        throw err;
    }
};