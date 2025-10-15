function auth(language) {
    Alpine.data("auth", () => ({
        firstName: '',
        lastName: '',
        email: '',
        userName: null,
        password: '',
        confirmPassword: '',
        enableNotifications: false,
        qrCodeSrc: '',
        twoFactorCode: '',
        isPersistent: false,
        isBusy: false,
        errorMessage: '',

        getQRCode: async function () {
            this.isBusy = true;

            try {
                const token = window.localStorage.getItem('2fa_token');
                const response = await getQRCodeAsync(token, language);

                if (response.status === 400) {

                }
                else {
                    const blob = await response.blob();
                    this.qrCodeSrc = URL.createObjectURL(blob);
                }
            }
            catch (error) {
                this.errorMessage = error.message;
            }
            finally {
                this.isBusy = false;
            }
        },

        login: async function () {
            this.isBusy = true;

            try {
                const response = await loginAsync(this.email, this.password, this.isPersistent, language);
                const content = await response.json();

                this.errorMessage = GetErrorMessage(response.status, content);
                if (this.errorMessage == null) {
                    if (content.token.startsWith('eyJ')) {
                        setAuthCookie('jwtBearer', content.token, this.isPersistent);
                    }
                    else {
                        window.localStorage.setItem('2fa_token', content.token);
                        window.location.href = '/Account/TwoFactorCode';
                    }
                }
            }
            catch (error) {
                this.errorMessage = error.message;
            }
            finally {
                this.isBusy = false;
            }
        },

        logout: async function () {
            this.isBusy = true;

            try {
                const response = await logoutAsync(language);
                const content = await content.json();

                this.errorMessage = GetErrorMessage(response.status, content);
                if (this.errorMessage == null) {
                    window.location.href = '/';
                }
            }
            catch (error) {
                this.errorMessage = error.message;
            }
            finally {
                this.isBusy = false;
            }
        },

        next: function () {
            window.location.href = '/Account/ValidateTwoFactor';
        },

        register: async function () {
            this.isBusy = true;

            try {
                const response = await registerAsync(this.firstName, this.lastName, this.email, this.password, this.userName, this.enableNotifications, language);
                const content = await response.json();

                this.errorMessage = GetErrorMessage(response.status, content);
                if (this.errorMessage == null) {
                    window.location.href = '/';
                }
            }
            catch (error) {
                this.errorMessage = error.message;
            }
            finally {
                this.isBusy = false;
            }
        },

        validateTwoFactor: async function () {
            this.isBusy = true;

            try {
                const response = await validateTwoFactorAsync(this.twoFactorCode, language);
                const content = await response.json();

                this.errorMessage = GetErrorMessage(response.status, content);
                if (this.errorMessage == null) {
                    window.localStorage.removeItem('2fa_token');
                    window.localStorage.setItem('access_token', content.token);
                    window.location.href = '/';
                }
            }
            catch (error) {
                this.errorMessage = error.message;
            }
            finally {
                this.isBusy = false;
            }
        }
    }));
}

async function getQRCodeAsync(token, language) {
    const response = await fetch(`/api/auth/qrcode?token=${token}`, {
        method: "GET",
        headers: {
            "Accept-Language": language
        }
    });

    return response;
}

async function loginAsync(email, password, isPersistent, language) {
    const request = {
        email: email,
        password: password,
        isPersistent: isPersistent
    };

    const response = await fetch('/api/auth/login', {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept-Language": language
        },
        body: JSON.stringify(request)
    });

    return response;
}

async function logoutAsync(language) {
    const response = await fetch('/api/auth/logout', {
        method: "POST",
        headers: {
            "Accept-Language": language
        }
    });

    return response;
}

async function registerAsync(firstName, lastName, email, password, userName, enableNotifications, language) {
    const request = {
        firstName: firstName,
        lastName: lastName,
        email: email,
        password: password,
        userName: userName,
        enableNotifications: enableNotifications
    };

    const response = await fetch('/api/auth/register', {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            "Accept-Language": language
        },
        body: JSON.stringify(request)
    });

    return response;
}