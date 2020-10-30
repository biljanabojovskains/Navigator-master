-- Table: public.roles

-- DROP TABLE public.roles;

CREATE TABLE public.roles
(
    role_id integer NOT NULL DEFAULT nextval('roles_role_id_seq'::regclass),
    rolename text COLLATE pg_catalog."default" NOT NULL,
    rolename_mk text COLLATE pg_catalog."default",
    CONSTRAINT roles_pkey PRIMARY KEY (role_id)
)
WITH (
    OIDS = FALSE
)
TABLESPACE pg_default;

ALTER TABLE public.roles
    OWNER to postgres;