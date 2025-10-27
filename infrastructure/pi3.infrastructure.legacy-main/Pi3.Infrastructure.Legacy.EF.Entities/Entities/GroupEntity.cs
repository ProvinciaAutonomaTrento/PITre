// SPDX-FileCopyrightText: 2025 Provincia Autonoma di Trento <https://www.provincia.tn.it>
// SPDX-License-Identifier: EUPL-1.2
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pi3.Infrastructure.Legacy.EF.Entities
{
    public class GroupEntity
    {
        public long SYSTEM_ID { get; set; }
        public string? GROUP_ID { get; set; }
        public string? NETWORK_ID { get; set; }
        public string? GROUP_NAME { get; set; }
        public long? PROFILE_DEFAULTS { get; set; }
        public string? DISABLED { get; set; }
        public string? ALLOW_LOGIN { get; set; }
        public long? UNIV_ACCESS { get; set; }
        public string? DELETE_VERSIONS { get; set; }
        public string? EDIT_PREVIOUS_VER { get; set; }
        public long? MAX_VERSIONS { get; set; }
        public long? MAX_SUBVERSIONS { get; set; }
        public string? NEW_VERSIONS { get; set; }
        public string? SAVE_TO_REM_LIB { get; set; }
        public string? PRECONNECT_LIBS { get; set; }
        public string? NV_AUTHOR_EDIT { get; set; }
        public string? NV_ENTERED_BY { get; set; }
        public string? NV_BILLABLE { get; set; }
        public string? DISPLAY_VER_LIST { get; set; }
        public string? MV_DOCS_IF_CHNG { get; set; }
        public string? CHECKOUT { get; set; }
        public string? OTHER_CHECKIN { get; set; }
        public string? CHECKIN_REMINDER { get; set; }
        public string? RESET_STATUS { get; set; }
        public string? COPY_IN_USE { get; set; }
        public string? TEMPLATE_MANAGER { get; set; }
        public string? MASS_UPD_PROFILES { get; set; }
        public string? AUTO_LOGIN { get; set; }
        public string? VIEW_UNSECURED { get; set; }
        public long? PROMPT_PAGES { get; set; }
        public string? NONBILL_PAGES { get; set; }
        public string? DEFAULT_PAGES { get; set; }
        public string? GET_EDIT_INFO { get; set; }
        public string? VISIT_AUTHOR_EDIT { get; set; }
        public string? VISIT_ENTERED_BY { get; set; }
        public string? LCP_RUN { get; set; }
        public string? EDIT_VTS { get; set; }
        public string? EDIT_LIBPARMS { get; set; }
        public string? EDIT_WS_PARAMS { get; set; }
        public string? EDIT_USER_DEFAULTS { get; set; }
        public string? MANAGE_GROUPS { get; set; }
        public string? DI_RUN { get; set; }
        public string? MBLINST_RUN { get; set; }
        public string? DI_MANAGE { get; set; }
        public string? CR_RUN { get; set; }
        public string? SM_RUN { get; set; }
        public string? DD_RUN { get; set; }
        public string? DB_EDIT { get; set; }
        public string? DBI_RUN { get; set; }
        public string? INDEXER_RUN { get; set; }
        public string? INTERCHANGE_RUN { get; set; }
        public string? PROFSEC { get; set; }
        public string? ALLOW_DOC_DELETE { get; set; }
        public string? ALLOW_CONTENT_DEL { get; set; }
        public string? ALLOW_QUEUE_DEL { get; set; }
        public long? PROFILE_FORM { get; set; }
        public long? HITLIST_FORM { get; set; }
        public string? ACL_DEFAULTS { get; set; }
        public string? REMOVE_MON_LIST { get; set; }
        public string? DISPLAY_MON_LIST { get; set; }
        public string? WARN_SECURE { get; set; }
        public string? ONLY_READONLY { get; set; }
        public string? FORCE_CHECKIN { get; set; }
        public string? MBL_EDITCOPY { get; set; }
        public string? MBL_OVERWRITE { get; set; }
        public string? MIN_DISKFREE { get; set; }
        public string? AUTOCLEAN { get; set; }
        public long? DEF_SHAD_RETENTION { get; set; }
        public string? SHADOW_DOCS { get; set; }
        public string? SHADOW_PROFILES { get; set; }
        public string? SHADOW_SEC_DOCS { get; set; }
        public string? DEFAULT_FT_INDEX { get; set; }
        public string? DISABLE_NATIVE { get; set; }
        public string? MAKE_READ_ONLY { get; set; }
        public string? REMOVE_READ_ONLY { get; set; }
        public string? MAKE_VER_READONLY { get; set; }
        public string? MAKE_VER_WRITABLE { get; set; }
        public string? PUBLISH_VERSION { get; set; }
        public string? UNPUBLISH_VERSION { get; set; }
        public string? DATE_FORMAT { get; set; }
        public string? TIME_FORMAT { get; set; }
        public long? ITEM_MAX { get; set; }
        public long? PAGE_MAX { get; set; }
        public string? DEFAULT_VIEWER { get; set; }
        public string? FRONTEND_PROFILE { get; set; }
        public string? MANAGE_PRF { get; set; }
        public string? MANAGE_CYD { get; set; }
        public long? DPACKAGE { get; set; }
        public string? ALLOW_APPINT { get; set; }
        public string? ALLOW_USRSETTINGS { get; set; }
        public string? ALLOW_DOC_CREATE { get; set; }
        public string? CREATE_FOLDER { get; set; }
        public string? ROOT_FOLDER { get; set; }
        public string? CREATE_RELATION { get; set; }
        public string? SHOW_RELATED { get; set; }
        public string? REMOVE_RELATION { get; set; }
        public string? ALLOW_NOTIF { get; set; }
        public string? ALLOW_PREVIEW { get; set; }
        public string? WARN_UPDATE_AVAIL { get; set; }

    }
}
