{
	"id":"12121",
	"temperature":3724,
	"humidity":37,
	"carbonDioxide":34,
	"acidBase":5,
	"nitrogen":1,
	"phosphorus":2,
	"potassium":3,
	"Illuminance":12,
	"deviceType":1,
	"remark":"12121",
	"information":"121212",
	"updateOn":"2020/12/28"
}

CREATE TABLE device_data
{
	id character(24) PRIMARY KEY,
	device_id character(24) NULL,
	sensor_type integer NOT NULL,
	data integer NULL,
	data_str character varying(128) NULL,
	unit_name character varying(32) NULL,
	remark character varying(512) NULL,
	information character varying(512) NULL,
	update_on timestamp NOT NULL
};


CREATE TABLE device_info
(
	id character(24) PRIMARY KEY,								
	device_code character varying(128) NULL,
	device_name character varying(128) NULL,
	serial_number character varying(128) NULL,
	longitude double precision NULL,
	latitude double precision NULL,
	config_json text NULL,
	description character varying(512) NULL,
	remark character varying(512) NULL,
	address character varying(1024) NULL,
	status integer NOT NULL,
	type integer NOT NULL,
	regist_token character(24) NOT NULL,
	modified_on timestamp NULL,
    regist_on timestamp NOT NULL
);

CREATE TABLE verification_code
(
	id serial PRIMARY KEY,
	member_id character(24) NULL,
	code character varying(8) NOT NULL,
	type integer NOT NULL DEFAULT 0,
	created_on timestamp NOT NULL,
	is_disabled boolean NOT NULL DEFAULT false,
	client_ip character varying(128) NULL,
	reason character varying(128) NULL,
	disabled_on timestamp NULL
);

CREATE TABLE member_account (
	account character varying(36) NOT NULL PRIMARY KEY, 
	member_id character(24) NOT NULL, 
	is_lockout boolean NOT NULL,  
	password character varying(36) NOT NULL, 
	password_format integer NOT NULL, 
	password_salt varchar(254)
);

CREATE TABLE member_token
(
	token character(24) PRIMARY KEY,
	member_id character(24) NOT NULL,
	client_ip character varying(64) NULL,
	client_type integer NOT NULL DEFAULT 0,
	is_disabled boolean NOT NULL Default False,
	reason character varying(256) NULL,
	disabled_on timestamp NULL,
	created_on timestamp NOT NULL
);
CREATE TABLE member_login_log (
	id serial PRIMARY KEY, 
	account character varying(36) NOT NULL, 
	password character varying(36) NOT NULL ,
	check_on timestamp NOT NULL,
	is_pass boolean NOT NULL DEFAULT false,
	reason character varying(256)  NULL,
	client_ip character varying(64) NULL,
	pass_on  timestamp
);

CREATE TABLE member_info (
	id character(24) primary key ,
	realname character varying(128) NUll,
	sex int NOT NULL Default 0,
	alias character varying(128) NULL, 
	avatar character varying(256) NULL,
	telephone character varying(36) NULL,
	mobile_phone character varying(36) NOT NULL, 
	level int NOT NULL DEFAULT 0,
	type int NOT NULL DEFAULT 0,
	main_member_id character(24) NULL,
	available_money numeric(36, 2) NOT NULL DEFAULT 0, 
	used_money numeric(36,2) NOT NULL DEFAULT 0,
	frozen_money numeric(36, 2) NOT NULL DEFAULT 0, 
	available_points numeric(36, 2) NOT NULL DEFAULT 0, 
	used_points numeric(36,2) NOT NULL DEFAULT 0,
	frozen_points numeric(36, 2) NOT NULL DEFAULT 0, 
	email character varying(256), 
	divisions_full_name  character varying(128) NULL,
	address character varying(256) NULL,
	birthday date NUll, 
	id_card character varying(36), 
	referee_id character(24) NULL,
	has_wechat boolean NOT NULL DEFAULT false,
	is_follow_official_account boolean NOT NULL DEFAULT false,			   ----是否关注公众号
	subscribe_on timestamp NULL,
	regist_code character(24) NULL,
	remark character varying(1024) NUll,
	created_on timestamp NOT NULL, 
	modified_on timestamp
);
