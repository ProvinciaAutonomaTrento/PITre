// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
window.fetch = function (fetch) {
        return function () {
            var req = arguments[1];
            if(req.headers["X-Requested-With"]) {
                delete req.headers["X-Requested-With"];
            }
            return fetch.apply(window, arguments);
        };
    }(window.fetch);