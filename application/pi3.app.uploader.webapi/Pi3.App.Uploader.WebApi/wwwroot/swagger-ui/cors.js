// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: AGPL-3.0-or-later
window.fetch = function (fetch) {
        return function () {
            var req = arguments[1];
            if(req.headers["X-Requested-With"]) {
                delete req.headers["X-Requested-With"];
            }
            return fetch.apply(window, arguments);
        };
    }(window.fetch);